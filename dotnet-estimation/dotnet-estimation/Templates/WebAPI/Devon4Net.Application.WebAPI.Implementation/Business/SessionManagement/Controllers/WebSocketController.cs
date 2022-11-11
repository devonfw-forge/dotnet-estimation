using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Infrastructure.Logger.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Controllers
{
    [Route("[controller]")]
    public class WebSocketController : Controller
    {
        public IWebSocketHandler WebsocketHandler { get; }

        public WebSocketController(IWebSocketHandler websocketHandler)
        {
            WebsocketHandler = websocketHandler;
        }

        //TODO: Add Autorization via Token  ( insert Handle(token.id as ConcurrentDictionary ID String))
        [HttpGet("/ws")]
        public async Task Connect()
        {
            var context = ControllerContext.HttpContext;
            var isSocketRequest = context.WebSockets.IsWebSocketRequest;

            if (isSocketRequest)
            {
                WebSocket websocket = await context.WebSockets.AcceptWebSocketAsync();

                await WebsocketHandler.Handle(Guid.NewGuid(), websocket);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }
    }
}
