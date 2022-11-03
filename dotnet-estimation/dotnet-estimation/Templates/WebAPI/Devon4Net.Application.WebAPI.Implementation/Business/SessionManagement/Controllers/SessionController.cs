using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Devon4Net.Infrastructure.Logger.Logging;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Exceptions;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors("CorsPolicy")]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService SessionService)
        {
            _sessionService = SessionService;
        }

        [HttpPut]
        [AllowAnonymous]
        [ProducesResponseType(typeof(bool), StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("/estimation/v1/session/{id:long}/invalidate")]
        public async Task<IActionResult> InvalidateSession(long id)
        {
            Devon4NetLogger.Debug($"Put-Request to invalidate session with id: {id}");

            try
            {
                return Accepted(await _sessionService.InvalidateSession(id));
            }
            catch (Exception exception)
            {
                return exception switch
                {
                    NotFoundException _ => NotFound(),
                    InvalidSessionException _ => BadRequest(),
                    _ => StatusCode(500)
                };
            }
        }
    }
}