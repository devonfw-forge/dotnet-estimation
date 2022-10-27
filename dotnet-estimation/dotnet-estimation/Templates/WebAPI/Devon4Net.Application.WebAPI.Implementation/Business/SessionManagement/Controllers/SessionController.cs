using System.ComponentModel.DataAnnotations;
using Devon4Net.Infrastructure.Logger.Logging;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dto;
namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors("CorsPolicy")]
    
    public class SessionController: ControllerBase
    {
        //private readonly ISessionService _sessionService;
        /*
        public SessionController( ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
        */
        


        [HttpPost]
        [Route("/session")]
        [ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult CreateSession(SessionDto sessionDto)
        {
            SessionDto session = new SessionDto();
            session.Id = 788;
            session.InviteToken = "invitetoken788";
            
            return StatusCode(StatusCodes.Status201Created, session);
        }
    }

}