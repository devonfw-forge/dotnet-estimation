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

            try
            {
                var (isValid, tasks) = await _sessionService.GetStatus(id);

                Devon4NetLogger.Debug($"Session is valid: {isValid}");

                var statusResult = new StatusDto
                {
                    IsValid = isValid,
                    Tasks = tasks.Select(item => TaskConverter.ModelToDto(item)).ToList()
                };

                return new ObjectResult(JsonConvert.SerializeObject(statusResult));
            }
            catch (Exception exception)
            {
                Devon4NetLogger.Debug($"Exception thrown: {exception.Message}");

                return exception switch
                {
                    NotFoundException _ => NotFound(),
                    _ => StatusCode(500),
                };
            }
        }
    }
}