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
using ErrorOr;
using Devon4Net.Infrastructure.Logger.Logging;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Exceptions;

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

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(StatusDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("/estimation/v1/session/{id:long}/status")]
        public async Task<IActionResult> GetSessionStatus(long id)
        {
            Devon4NetLogger.Debug($"Get-Request for session status with id: {id}");

                var ErrorOrStatus = await _sessionService.GetStatus(id);

                Devon4NetLogger.Debug($"Session is valid: {ErrorOrStatus.Value.Item1}");
                Devon4NetLogger.Debug($"{ErrorOrStatus.FirstError.Description}");
                var statusResult = new StatusDto
                {
                    IsValid = ErrorOrStatus.Value.Item1,
                    Tasks = ErrorOrStatus.Value.Item2.Select(item => TaskConverter.ModelToDto(item)).ToList(),
                    Users = ErrorOrStatus.Value.Item3.Select(item => TaskConverter.ModelToDto(item)).ToList(),
                };

                return new ObjectResult(JsonConvert.SerializeObject(statusResult));
            }

        }
    }
}
