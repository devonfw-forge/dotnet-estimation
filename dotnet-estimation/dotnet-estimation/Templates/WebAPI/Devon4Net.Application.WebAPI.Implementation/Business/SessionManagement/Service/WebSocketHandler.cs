using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    public class WebSocketHandler : IWebSocketHandler
    {
        public readonly struct WebSocketConnection
        {
            public readonly string Id { get; init;}

            public readonly WebSocket Value { get; init; }
        }
        private ConcurrentDictionary<long, ConcurrentBag<WebSocketConnection>> _sessions = new ConcurrentDictionary<long, ConcurrentBag<WebSocketConnection>>();

        public async Task Handle(Guid id, WebSocket webSocket, long sessionId)
        {
            var socketConnection = new WebSocketConnection { Id = id.ToString(), Value = webSocket };
            _sessions.AddOrUpdate(sessionId,
                id => new ConcurrentBag<WebSocketConnection>() { socketConnection },
                (id, existingBag) =>
                {
                    existingBag.Add(socketConnection);
                    return existingBag;
                });

            while (webSocket.State == WebSocketState.Open)
            {
                var message = await ReceiveMessage(id, webSocket);
                if (message != null)
                    await SendMessageToSockets(message, sessionId);
            }
        }

        public async Task<string> ReceiveMessage(Guid id, WebSocket webSocket)
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
                foreach (var connection in _sessions[sessionId])
                {
                    if (connection.Value.State == WebSocketState.Open)
                    {
                        var bytes = Encoding.Default.GetBytes(message);
                        var arraySegment = new ArraySegment<byte>(bytes);
                        await connection.Value.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
        }

        public async Task Send<T>(Message<T> message, long sessionId)
        {
            await SendMessageToSockets(JsonConvert.SerializeObject(message, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), sessionId);
        }
    }
}
