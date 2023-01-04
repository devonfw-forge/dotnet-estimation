using System.Net.WebSockets;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    public interface IWebSocketHandler
    {
        Task Handle(string token, WebSocket webSocket, long sessionId);
        Task<string> ReceiveMessage(string id, WebSocket webSocket);
        Task SendMessageToSockets(string message, long sessionId);
        Task Send<T>(Message<T> message, long sessionId);
    }
}