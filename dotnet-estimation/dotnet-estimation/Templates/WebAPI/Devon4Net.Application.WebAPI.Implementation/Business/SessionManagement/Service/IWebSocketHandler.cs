using System.Net.WebSockets;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    public interface IWebSocketHandler
    {
        Task Handle(Guid id, WebSocket webSocket);
        Task<string> ReceiveMessage(Guid id, WebSocket webSocket);
        Task SendMessageToSockets(string message);
    }
}