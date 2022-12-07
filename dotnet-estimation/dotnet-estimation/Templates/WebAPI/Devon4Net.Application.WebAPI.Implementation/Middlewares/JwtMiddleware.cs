using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using Devon4Net.Infrastructure.JWT.Handlers;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using System.Security.Claims;

namespace Devon4Net.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJwtHandler _jwtHandler;

        public JwtMiddleware(RequestDelegate next, IJwtHandler jwtHandler)
        {
            _next = next;
            _jwtHandler = jwtHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            Devon4Net.Infrastructure.Logger.Logging.Devon4NetLogger.Debug("Triggered JwtMiddleware");

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            Devon4Net.Infrastructure.Logger.Logging.Devon4NetLogger.Debug(token);

            var userClaims = _jwtHandler.GetUserClaims(token).ToList();

            // Enum.TryParse(_jwtHandler.GetClaimValue(userClaims, ClaimTypes.Role), out Application.WebAPI.Implementation.Domain.Entities.Role role);

            // Return result with claims values
            var result = new AuthenticatedUserDto
            {
                Id = _jwtHandler.GetClaimValue(userClaims, ClaimTypes.NameIdentifier),
                Username = _jwtHandler.GetClaimValue(userClaims, ClaimTypes.Name),
                //Role = role
            };

            // attach user to context on successful jwt validation
            context.Items["user"] = result;
            // context.Request. = new AuthenticatedUserDto { Id = "jaksdjkal", Username = "Helga" };

            await _next(context);
        }
    }
}