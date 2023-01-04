using Devon4Net.Infrastructure.Logger.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Devon4Net.Infrastructure.JWT.Handlers;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    public class WebSocketHandler : IWebSocketHandler
    {
        private ConcurrentDictionary<long, ConnectionManager> _sessions = new ConcurrentDictionary<long, ConnectionManager>();

        private readonly ISessionService _sessionService;
        public IJwtHandler JwtHandler { get; }

        public WebSocketHandler(ISessionService sessionService, IJwtHandler jwtHandler)
        {
            _sessionService = sessionService;
            JwtHandler = jwtHandler;
        }

        public async Task Handle(string token, WebSocket webSocket, long sessionId)
        {
            var userClaims = JwtHandler.GetUserClaims(token).ToList();
            var id = JwtHandler.GetClaimValue(userClaims, ClaimTypes.NameIdentifier);
            var username = JwtHandler.GetClaimValue(userClaims, ClaimTypes.Name);
            var roleString = JwtHandler.GetClaimValue(userClaims, ClaimTypes.Role);
            var joinedUser = new UserDto
            {
                Id = id,
                Username = username,
                Role = (Role)Enum.Parse(typeof(Role), roleString),
                Online = true,
            };

            // Update the session lobby with the newly joined client
            UpdateSession(sessionId, id, webSocket);
            _sessions.TryGetValue(sessionId, out var currentConnectionsManager);
            Devon4NetLogger.Debug($"Amount of current Sockets in Lobby: {currentConnectionsManager.GetAll().Count}\nFollowing client joined: {id}");
            
            // Notify all lobby users about the recently joined user.
            var currentlyJoinedClient = new Message<UserDto>
            {
                Type = MessageType.UserJoined,
                Payload = joinedUser,
            };
            var availableClients = currentConnectionsManager.GetAvailableSocketIds();
            var updateClients = new Message<UpdateDto>
            {
                Type = MessageType.UserRefreshed,
                Payload = new UpdateDto
                {
                    AvailableClients = availableClients,
                }
            };
            await Send(currentlyJoinedClient, sessionId);
            await Send(updateClients, sessionId);


            while (webSocket.State == WebSocketState.Open)
            {
                var message = await ReceiveMessage(id, webSocket);
                if (message != null)
                    await SendMessageToSockets(message, sessionId);
            }

            // Handle client disconnects
            if (webSocket.State == WebSocketState.CloseReceived || webSocket.State == WebSocketState.Closed)
            {
                Devon4NetLogger.Debug($"Handle Client Disconnect for {id}");
                var ClientReconnected = false;
                // If the client does not reconnect during 3 minutes, dispose the connection to clear resources.
                var Startpoint = System.Diagnostics.Stopwatch.StartNew();
                while (Startpoint.ElapsedMilliseconds < 1000*5*1)
                {
                    var currentAvailableClients = currentConnectionsManager.GetAvailableSocketIds();

                    if (currentAvailableClients.Any(clientId => clientId == id))
                    {
                        ClientReconnected = true;
                        break;
                    }
                }
                Startpoint.Stop();

                if (!ClientReconnected)
                {
                    await AbortWebSocket(sessionId, id);
                    Devon4NetLogger.Debug($"Amount of current Sockets in our Lobby: {currentConnectionsManager.GetAll().Count}\nDeleted following client: {id}");
                    
                    // Notify session users about the disconnected user.
                    var RemainingClientIds = currentConnectionsManager.GetAvailableSocketIds();
                    var updateRemainingClients = new Message<UpdateDto>
                    {
                        Type = MessageType.UserRefreshed,
                        Payload = new UpdateDto
                        {
                            AvailableClients = RemainingClientIds,
                        }
                    };
                    await Send(updateRemainingClients, sessionId);

                    // TODO: When are we going to delete the users from our database ?
                    // TODO: Add _userService and Remove User From our database here ?
                }
            }
        }

        public async Task<string> ReceiveMessage(string id, WebSocket webSocket)
        {
            var arraySegment = new ArraySegment<byte>(new byte[4096]);
            var receivedMessage = await webSocket.ReceiveAsync(arraySegment, CancellationToken.None);
            if (receivedMessage.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.Default.GetString(arraySegment).TrimEnd('\0');
                if (!string.IsNullOrWhiteSpace(message))
                    return $"<b>{id}</b>: {message}";
            }
            return null;
        }

        public async Task SendMessageToSockets(string message, long sessionId)
        {
            if (_sessions.ContainsKey(sessionId))
            {
                _sessions.TryGetValue(sessionId, out var connectionManager);
                var currentSockets = connectionManager.GetAllOpen();
                foreach (var connection in currentSockets)
                {
                    var bytes = Encoding.Default.GetBytes(message);
                    var arraySegment = new ArraySegment<byte>(bytes);
                    await connection.Value.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        public async Task Send<T>(Message<T> message, long sessionId)
        {
            await SendMessageToSockets(JsonConvert.SerializeObject(message, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), sessionId);
        }

        public async Task<bool> DeleteWebSocket(long sessionId, string clientId)
        {
            var deleted = await AbortWebSocket(sessionId, clientId);
            if (!deleted)
            {
                Devon4NetLogger.Debug($"The Websocket for client: {clientId} was NOT DELETED!");
            }
            return deleted;
        }

        private async Task<bool> AbortWebSocket(long sessionId, string clientId)
        {
            //ConnectionManager connectionManager;
            //Get the current Lobby of clients
            _sessions.TryGetValue(sessionId, out var connectionManager);

            //Delete the client with specified clientId, but keep an instance of the old lobby to compare with, when updating the sessions dictionary.
            var resultLobby = connectionManager;

            var deleted = resultLobby.RemoveSocket(clientId);
            //Update the sessions dictrionary, to free up resources of unused WebSockets
            _sessions.TryUpdate(sessionId, resultLobby, connectionManager);
            return deleted.Result;
        }

        private void UpdateSession(long sessionId, string clientId, WebSocket webSocket)
        {
            _sessions.AddOrUpdate(sessionId, sessionId =>
            {
                //No existing lobby for the given sessionId --> create a new lobby and insert the first client
                var lobby = new ConnectionManager(new ConcurrentDictionary<string, WebSocket>());
                lobby.AddSocket(clientId, webSocket);

                // Save the initial lobby for given sessionId to our sessions dictionary.
                _sessions.TryAdd(sessionId, lobby);
                return lobby;
            },
            (_, existingLobby) =>
            {
                //A lobby for the given sessionId exists --> extend the lobby by the recently joined client

                // True, if the client is connecting for the first time to the session.
                // False, if the client is trying to reconnect for example:
                // Website reload. Unintended Tabclose and reopen via history or webbrowser relaunch.
                var isAdded = existingLobby.AddSocket(clientId, webSocket);

                if (isAdded is false)
                {
                    Devon4NetLogger.Debug($"Trying to reconnect the known user with following id:\n {clientId}");
                    existingLobby.ReconnectClient(clientId, webSocket);
                }

                return existingLobby;
            });
        }
    }
}
