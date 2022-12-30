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
using Devon4Net.Authorization;

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
            var errorOrResult = await _sessionService.CreateSession(sessionDto);

            if (errorOrResult.IsError)
            {
                Devon4NetLogger.Debug(errorOrResult.FirstError.Description);
                return BadRequest();
            }

                return Ok(errorOrResult.Value);
            
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
            var errorOrResult = await _sessionService.InvalidateSession(id);

            if (errorOrResult.IsError)
            {
                Devon4NetLogger.Debug(errorOrResult.FirstError.Description);
                return BadRequest();
            }
            
            return Ok(errorOrResult.Value);
            
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

            var errorOrResult = await _sessionService.RemoveUserFromSession(sessionId, userId);

            if (errorOrResult.IsError)
            {
                Devon4NetLogger.Debug(errorOrResult.FirstError.Description);
                return BadRequest();
            }

            return new ObjectResult(JsonConvert.SerializeObject(errorOrResult.Value));
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
        [Route("/estimation/v1/session/{inviteToken}/entry")]
        public async Task<ActionResult<UserDto>> AddUserToSession(string inviteToken, [FromBody] JoinSessionDto joinDto)
        {
            Devon4NetLogger.Debug("Executing AddUserToSession from controller SessionController");

            var (username, desiredRole) = joinDto;

            var errorOrResult = await _sessionService.AddUserToSession(inviteToken, username, desiredRole);

            if (errorOrResult.IsError)
            {
                Devon4NetLogger.Debug(errorOrResult.FirstError.Description);
                return BadRequest();
            }

            var (sessionId, userId, _, role, token) = errorOrResult.Value.Item2;

            Message<UserDto> Message = new Message<UserDto>
            {
                Type = MessageType.UserJoined,
                Payload = new UserDto { Id = userId, Role = role, Token = token, Username = username },
            };

            await _webSocketHandler.Send(Message, sessionId);

            return new ObjectResult(JsonConvert.SerializeObject(errorOrResult.Value.Item2));
        }

        [HttpPost]
        [TransientAuthorizationAttribute]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("/estimation/v1/session/{sessionId:long}/task")]
        public async Task<ActionResult> AddTask(long sessionId, [FromBody] TaskDto task)
        {
            Devon4NetLogger.Debug($"{sessionId}");

            var (userId, _) = Request.HttpContext.Items["user"] as AuthenticatedUserDto;

            Devon4NetLogger.Debug($"{userId}");

            var errorOrResult = await _sessionService.AddTaskToSession(sessionId, userId, task);

            if (errorOrResult.IsError)
            {
                Devon4NetLogger.Debug(errorOrResult.FirstError.Description);
                return BadRequest();
            }

            Message<TaskDto> Message = new Message<TaskDto>
            {
                Type = MessageType.TaskCreated,
                Payload = errorOrResult.Value.Item2
            };

            await _webSocketHandler.Send(Message, sessionId);

            return Ok();

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

           
            var errorOrResult = await _sessionService.DeleteTask(sessionId, taskId);

            if (errorOrResult.IsError)
            {
                Devon4NetLogger.Debug(errorOrResult.FirstError.Description);
                return BadRequest();
            }

            Message<string> message = new Message<string>
            {
                Type = MessageType.TaskDeleted,
                Payload = taskId
            };
            await _webSocketHandler.Send(message, sessionId);
            return Ok();

         
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

            var errorOrResult = await _sessionService.AddNewEstimation(sessionId, taskId, voteBy, complexity);

            if (errorOrResult.IsError)
            {
                Devon4NetLogger.Debug(errorOrResult.FirstError.Description);
                return BadRequest();
            }

            await _webSocketHandler.Send(new Message<EstimationDto> { Type = MessageType.EstimationAdded, Payload = estimationDto }, sessionId);

            return StatusCode(StatusCodes.Status201Created, errorOrResult.Value);
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

            var errorOrResult = await _sessionService.ChangeTaskStatus(sessionId, statusChange);

            if (errorOrResult.IsError)
            {
                Devon4NetLogger.Debug(errorOrResult.FirstError.Description);
                return BadRequest();
            }

            await _webSocketHandler.Send(new Message<List<TaskStatusChangeDto>> { Type = MessageType.TaskStatusModified, Payload = errorOrResult.Value.Item2 }, sessionId);

            return Ok(errorOrResult.Value.Item2);

        }
    }
}
