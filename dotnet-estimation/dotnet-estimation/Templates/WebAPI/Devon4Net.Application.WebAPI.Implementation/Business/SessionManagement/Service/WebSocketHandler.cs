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
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public async Task Handle(Guid id, WebSocket webSocket)
        {
            _sockets.TryAdd(id.ToString(), webSocket);
            await SendMessageToSockets($"User with id <b>{id}</b> has joined the chat");

            while (webSocket.State == WebSocketState.Open)
            {
                var message = await ReceiveMessage(id, webSocket);
                if (message != null)
                    await SendMessageToSockets(message);
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

        //TODO: Add Generic Message Type 
        public async Task SendMessageToSockets(string message)
        {
            foreach (var (id, socket) in _sockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    var bytes = Encoding.Default.GetBytes(message);
                    var arraySegment = new ArraySegment<byte>(bytes);
                    await socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
