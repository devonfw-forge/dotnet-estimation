using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using Devon4Net.Infrastructure.JWT.Handlers;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using System.Security.Claims;
using Devon4Net.Infrastructure.LiteDb.Repository;

namespace Devon4Net.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJwtHandler _jwtHandler;
        private readonly ILiteDbRepository<Devon4Net.Application.WebAPI.Implementation.Domain.Entities.User> _userRepository;

        public JwtMiddleware(RequestDelegate next, IJwtHandler jwtHandler, ILiteDbRepository<Devon4Net.Application.WebAPI.Implementation.Domain.Entities.User> userRepository)
        {
            _next = next;
            _jwtHandler = jwtHandler;
            _userRepository = userRepository;
        }

        public async Task Invoke(HttpContext context)
        {
            Devon4Net.Infrastructure.Logger.Logging.Devon4NetLogger.Debug("Triggered JwtMiddleware");

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            Devon4Net.Infrastructure.Logger.Logging.Devon4NetLogger.Debug(token);

            if (token is null)
            {
                Devon4Net.Infrastructure.Logger.Logging.Devon4NetLogger.Debug("Token is null!");
            }

            var userClaims = _jwtHandler.GetUserClaims(token).ToList();
            // Enum.TryParse(_jwtHandler.GetClaimValue(userClaims, ClaimTypes.Role), out Application.WebAPI.Implementation.Domain.Entities.Role role);

            var userId = _jwtHandler.GetClaimValue(userClaims, ClaimTypes.NameIdentifier);

            var userEntity = _userRepository.Get(LiteDB.Query.EQ("_id", userId)).First();

            if (userEntity is null)
            {
                Devon4Net.Infrastructure.Logger.Logging.Devon4NetLogger.Debug("UserEntity is null!");
            }

            // attach user to context on successful jwt validation
            context.Items["user"] = new AuthenticatedUserDto { Id = userEntity.Id, Username = userEntity.Username };

            await _next(context);
        }
    }
}