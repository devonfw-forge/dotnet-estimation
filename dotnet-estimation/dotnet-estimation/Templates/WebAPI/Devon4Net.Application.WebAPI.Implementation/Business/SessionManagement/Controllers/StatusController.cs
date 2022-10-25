using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Converters;

using Devon4Net.Infrastructure.Logger.Logging;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors("CorsPolicy")]
    public class StatusController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public StatusController(ISessionService SessionService)
        {
            _sessionService = SessionService;
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(StatusDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("/estimation/v1/status")]
        public async Task<IActionResult> GetSessionStatus(long requestedSessionId)
        {
            Devon4NetLogger.Debug($"{requestedSessionId}");

            if (requestedSessionId == null)
            {
                return StatusCode(400);
            }

            var statusResult = new StatusDto { IsValid = false, CurrentTask = null };

            Session sessionResult = await _sessionService.GetSession(requestedSessionId);

            // TODO: catch if session can not be found! (too simple!)
            if (sessionResult == null) 
            {
                return StatusCode(404);
            }

            bool sessionIsValid = sessionResult.ExpiresAt > DateTime.Now;

            statusResult.IsValid = sessionIsValid;

            if (!sessionIsValid)
            {
                return new ObjectResult(JsonConvert.SerializeObject(statusResult));
            }

            var openTasks = sessionResult.Tasks.Where(item => item.Status == Status.Open).ToList();

            if (openTasks.Any()) 
            {
                openTasks.Sort((x, y) => DateTime.Compare(x.CreatedAt, y.CreatedAt));

                var currentTask = openTasks.First();

                var taskDto = TaskConverter.ModelToDto(currentTask);

                statusResult.CurrentTask = taskDto;
            } 
            else
            {
                var suspendedTasks = sessionResult.Tasks.Where(item => item.Status == Status.Suspended).ToList();

                suspendedTasks.Sort((x, y) => DateTime.Compare(x.CreatedAt, y.CreatedAt));

                if (suspendedTasks.Any())
                {
                    var currentTask = suspendedTasks.First();

                    var taskDto = TaskConverter.ModelToDto(currentTask);

                    statusResult.CurrentTask = taskDto;
                }
            }

            return new ObjectResult(JsonConvert.SerializeObject(statusResult));
        }
    }
}