using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Collections.Concurrent;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    public class ConnectionManager : IConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> _sockets;

        public ConnectionManager(ConcurrentDictionary<string, WebSocket> sockets)
        {
            _sockets = sockets;
        }

        public WebSocket GetSocketById(string id)
        {
            return _sockets.FirstOrDefault(p => p.Key == id).Value;
        }

        public ConcurrentDictionary<string, WebSocket> GetAll()
        {
            return _sockets;
        }

        public ConcurrentDictionary<string, WebSocket> GetAllOpen()
        {
            ConcurrentDictionary<string, WebSocket> availableSockets = new ConcurrentDictionary<string, WebSocket>();
            foreach (var client in _sockets)
            {
                if (client.Value.State.Equals(WebSocketState.Open))
                {
                    availableSockets.TryAdd(client.Key, client.Value);
                }
            }
            return availableSockets;
        }

        public ConcurrentDictionary<string, WebSocket> GetAllClosed()
        {
            ConcurrentDictionary<string, WebSocket> disconnectedSockets = new ConcurrentDictionary<string, WebSocket>();
            foreach (var client in _sockets)
            {
                if (client.Value.State.Equals(WebSocketState.Closed) || client.Value.State.Equals(WebSocketState.CloseReceived))
                {
                    disconnectedSockets.TryAdd(client.Key, client.Value);
                }
            }
            return disconnectedSockets;
        }

        public Message<List<string>> GetAvailableSocketIds()
        {
            var availableSockets = GetAllOpen();
            var loggedInUsers = new List<string>();

            foreach (var client in availableSockets)
            {
                loggedInUsers.Add(client.Key);
            }

            Message<List<string>> availableClientIds = new Message<List<string>>
            {
                Type = MessageType.UserJoined,
                Payload = loggedInUsers,
            };
            return availableClientIds;
        }

        public string GetId(WebSocket socket)
        {
            return _sockets.FirstOrDefault(p => p.Value == socket).Key;
        }

        public bool AddSocket(string id, WebSocket socket)
        {
            return _sockets.TryAdd(id, socket);
        }

        public bool ReconnectClient(string id, WebSocket socket)
        {
            var knownSocketConnection = GetSocketById(id);
            if (knownSocketConnection.State != WebSocketState.Open && knownSocketConnection.State != WebSocketState.Connecting)
            {
                return _sockets.TryUpdate(id, socket, knownSocketConnection);
            }
            return false;
        }

        public async Task<bool> RemoveSocket(string id)
        {
            WebSocket socket;
            var isRemoved = _sockets.TryRemove(id, out socket);

            await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                    statusDescription: "Closed by the ConnectionManager",
                                    cancellationToken: CancellationToken.None);
            socket.Abort();
            socket.Dispose();
            return isRemoved;
        }
    }
}
