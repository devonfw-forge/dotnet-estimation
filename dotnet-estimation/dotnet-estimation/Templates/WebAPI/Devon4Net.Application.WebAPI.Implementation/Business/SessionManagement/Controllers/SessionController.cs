using Newtonsoft.Json;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Exceptions;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using Devon4Net.Infrastructure.Logger.Logging;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Devon4Net.Infrastructure.JWT.Common.Const;
using System.Net;
using Task = System.Threading.Tasks.Task;
using System.Net.WebSockets;
using LiteDB;

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
        private readonly IWebSocketHandler _webSocketHandler;

        public SessionController(ISessionService SessionService, IWebSocketHandler webSocketHandler)
        {
            _sessionService = SessionService;
            _webSocketHandler = webSocketHandler;
        }

        /// <summary>
        /// Creates a session
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/estimation/v1/session/newSession")]
        [ProducesResponseType(typeof(ResultCreateSessionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateSession(SessionDto sessionDto)
        {
            Devon4NetLogger.Debug($"Create session that will expire at {sessionDto.ExpiresAt}");
            try
            {
                return Ok(await _sessionService.CreateSession(sessionDto));
            }
            catch (Exception exception)
            {
                return exception switch
                {
                    InvalidExpiryDateException _ => BadRequest(),
                    _ => StatusCode(500)
                };
            }
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("/estimation/v1/session/{sessionId:long}/task")]
        public async Task<ActionResult> AddTask(long sessionId, [FromBody] TaskDto task)
        {
            var (finished, taskDto) = await _sessionService.AddTaskToSession(sessionId, task);

            if (finished)
            {
                Message<TaskDto> Message = new Message<TaskDto>
                {
                    Type = MessageType.TaskCreated,
                    Payload = taskDto
                };

                await _webSocketHandler.Send(Message, sessionId);

                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("/estimation/v1/session/{sessionId:long}/task/{taskId}")]
        public async Task<ActionResult> DeleteTask(long sessionId, string taskId)
        {
            Devon4NetLogger.Debug($"Delete-Request to delete task with id: {taskId} from session with id: {sessionId}");

            try
            {
                var finished = await _sessionService.DeleteTask(sessionId, taskId);

                if (finished)
                {
                    Message<string> message = new Message<string>
                    {
                        Type = MessageType.TaskDeleted,
                        Payload = taskId
                    };
                    await _webSocketHandler.Send(message, sessionId);
                    return Ok();
                }

                return BadRequest();
            }
            catch (Exception exception)
            {
                return exception switch
                {
                    NotFoundException or TaskNotFoundException => NotFound(),
                    _ => StatusCode(500)
                };
            }
        }

        /// <summary>
        /// Add a Session Esstimation 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(EstimationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("/estimation/v1/session/{sessionId:long}/estimation")]
        public async Task<ActionResult<EstimationDto>> AddNewEstimation(long sessionId, [FromBody] EstimationDto estimationDto)
        {
            Devon4NetLogger.Debug("Executing AddNewEstimation from controller SessionController");

            var (taskId, voteBy, complexity) = estimationDto;

            var result = await _sessionService.AddNewEstimation(sessionId, taskId, voteBy, complexity);

            await _webSocketHandler.Send(new Message<EstimationDto> { Type = MessageType.EstimationAdded, Payload = estimationDto }, sessionId);

            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("/estimation/v1/session/{sessionId:long}/task/status")]
        public async Task<IActionResult> ChangeTaskStatus(long sessionId, [FromBody] TaskStatusChangeDto statusChange)
        {
            // Changing the status of a task requires other elements to be modified.
            // There can always be only one open or evaluated task at the same time.
            var (finished, modifiedTasks) = await _sessionService.ChangeTaskStatus(sessionId, statusChange);

            if (finished)
            {
                await _webSocketHandler.Send(new Message<List<TaskStatusChangeDto>> { Type = MessageType.TaskStatusModified, Payload = modifiedTasks }, sessionId);

                return Ok(modifiedTasks);
            }
            else
            {
                return BadRequest("Zero entries got updated due to errors. Please ensure the task exists.");
            }
        }
    }
}
