using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Exceptions;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Converters;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Infrastructure.Logger.Logging;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Devon4Net.Infrastructure.JWT.Common.Const;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dto;

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
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("/estimation/v1/session/{id:long}/invalidate")]
        public async Task<IActionResult> InvalidateSession(long id)
        {
            Devon4NetLogger.Debug($"Put-Request to invalidate session with id: {id}");

            try
            {
                return Ok(await _sessionService.InvalidateSession(id));
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

        /// <summary>
        /// Remove a Session user
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("/estimation/v1/session/{sessionId:long}/leaveSession/{userId}")]
        public async Task<IActionResult> RemoveUserFromSession(long sessionId, String userId)
        {
            Devon4NetLogger.Debug($"Put-Request for removing user with id: {userId} from session status with id: {sessionId}");

            try
            {
                var leaveResult = await _sessionService.RemoveUserFromSession(sessionId, userId);

                return new ObjectResult(JsonConvert.SerializeObject(leaveResult));
            }
            catch (Exception exception)
            {
                return exception switch
                {
                    NotFoundException _ => StatusCode(500),
                };
            }
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
            //Get claims
            var token = Request.Headers["Authorization"].ToString().Replace($"{AuthConst.AuthenticationScheme} ", string.Empty);

            if (string.IsNullOrEmpty(token)) return Unauthorized();

            Devon4NetLogger.Debug("Executing AddUserToSession from controller SessionController");
            var result = await _sessionService.AddUserToSession(sessionId, userDto.Id,
                userDto.Role).ConfigureAwait(false);
            return StatusCode(StatusCodes.Status201Created, result);
        }
    }
}