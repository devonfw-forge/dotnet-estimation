using System.ComponentModel.DataAnnotations;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Infrastructure.Logger.Logging;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dto;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Devon4Net.Infrastructure.JWT.Common.Const;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Controllers
{
    /// <summary>
    /// Session controller
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [EnableCors("CorsPolicy")]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sessionService"></param>
        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        /// <summary>
        /// Gets the entire list of sessions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<SessionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = AuthConst.AuthenticationScheme, Roles = $"Author,Moderator,Voter")]
        public async Task<ActionResult> GetSession(long id)
        {
            Devon4NetLogger.Debug("Executing GetSession from controller SessionController");
            return Ok(await _sessionService.GetSession(id).ConfigureAwait(false));
        }

        /// <summary>
        /// Add a Session user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = AuthConst.AuthenticationScheme, Roles = $"Author,Moderator,Voter")]
        [Route("/estimation/v1/session/{sessionId:long}/entry")]
        public async Task<ActionResult<UserDto>> AddUserToSession(long sessionId, UserDto userDto)
        {
            Devon4NetLogger.Debug("Executing AddUserToSession from controller SessionController");
            var result = await _sessionService.AddUserToSession(sessionId, userDto.Id,
                userDto.Role).ConfigureAwait(false);
            return StatusCode(StatusCodes.Status201Created, result);
        }
    }
}
