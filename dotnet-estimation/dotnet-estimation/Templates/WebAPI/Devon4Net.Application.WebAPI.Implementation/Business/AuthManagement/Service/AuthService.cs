using Devon4Net.Application.WebAPI.Implementation.Business.AuthManagement.Dto;
using Devon4Net.Infrastructure.JWT.Common.Const;
using Devon4Net.Infrastructure.JWT.Handlers;
using Devon4Net.Infrastructure.Logger.Logging;
using System.Security.Claims;

namespace Devon4Net.Application.WebAPI.Implementation.Business.AuthManagement.Service
{
    public class AuthService : IAuthService
    {
        private IJwtHandler _jwtHandler;
        public AuthService(IJwtHandler jwtHandler)
        {
            _jwtHandler = jwtHandler;
        }

        //TODO 1.1: Assign the User Role dynamically. (At the moment, every user gets the Voter role as default)
        public async Task<LoginResponse> getTokenUsername(string username)
        {
            var token = _jwtHandler.CreateJwtToken(new List<Claim> {
                new Claim(ClaimTypes.Role, "Voter"),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            });

            // TODO 1.2: Save the token inside a local database ??
            return new LoginResponse { Token = token };
        }

        public async Task<LoginResponse> login(string user, string password)
        {
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
            {
                return new LoginResponse { Token = "Neither the username nor the password can be empty" };
            }

            //TODO: check the database for the role status logic 
            var nameClaim = new Claim(ClaimTypes.Name, user);
            var roleClaim = new Claim(ClaimTypes.Role, "Voter");

            Devon4NetLogger.Debug("Executing Login from Service AuthService");
            var token = _jwtHandler.CreateJwtToken(new List<Claim> {
                new Claim(ClaimTypes.Role, AuthConst.DevonSampleUserRole),
                new Claim(ClaimTypes.Name,user),
                new Claim(ClaimTypes.NameIdentifier,Guid.NewGuid().ToString()),
            });
            return new LoginResponse { Token = token };
        }
    }
}
