﻿using System.Security.Claims;
using Devon4Net.Application.WebAPI.Implementation.Business.AuthManagement.Dto;
using Devon4Net.Application.WebAPI.Implementation.Business.AuthManagement.Service;
using Devon4Net.Infrastructure.JWT.Common.Const;
using Devon4Net.Infrastructure.JWT.Handlers;
using Devon4Net.Infrastructure.Logger.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Devon4Net.Application.WebAPI.Implementation.Business.AuthManagement.Controllers
{
    /// <summary>
    /// Auth controller sample.
    /// Please remember to avoid the use ob logic in a controller !!!
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private IJwtHandler JwtHandler { get; }
        private IAuthService _authService;

        /// <summary>
        /// Constructor with DI
        /// </summary>
        /// <param name="jwtHandler"></param>
        public AuthController(IJwtHandler jwtHandler, IAuthService authService)
        {
            JwtHandler = jwtHandler;
            _authService = authService;
        }


        /// <summary>
        /// Performs the join proces via the user flow
        /// </summary>
        /// <returns>LoginResponse class will provide the JWT token to securize the server calls</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("/v1/join/session")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> JoinSessionAsync(string user)
        {
            if(string.IsNullOrEmpty(user))
            {
                Devon4NetLogger.Debug("Session Join user did not provide an username from controller AuthController");
                return BadRequest("The username can not be empty");
            }
            else
            {
                Devon4NetLogger.Debug("Executing Login from controller AuthController");
                var result = await _authService.getTokenUsername(user);
                return Ok(result);
            }
        }
        /*
        /// <summary>
        /// Performs the login proces via the user/password flow
        /// This is only a sample. Please avoid any logic on the controller.
        /// </summary>
        /// <returns>LoginResponse class will provide the JWT token to securize the server calls</returns>
        [HttpPost]
        [HttpOptions]
        [AllowAnonymous]
        [Route("/v1/auth/login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async IActionResult Login(string user, string password)
        {
            var result = await _authService.login(user, password);

            if (result.Token == "" )
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("The user name of password can not be empty");
            }

            Devon4NetLogger.Debug("Executing Login from controller AuthController");

            var token = JwtHandler.CreateJwtToken(new List<Claim>
            {
                new Claim(ClaimTypes.Role, AuthConst.DevonSampleUserRole),
                new Claim(ClaimTypes.Name,user),
                new Claim(ClaimTypes.NameIdentifier,Guid.NewGuid().ToString()),
            });

            return Ok(new LoginResponse { Token = token });
        }
        */

        /// <summary>
        /// Provides the information related to the logged user
        /// This is only a sample. Please never put any logic on a controller
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpOptions]
        [Authorize(AuthenticationSchemes = AuthConst.AuthenticationScheme, Roles = $"Author,Moderator,Voter")]
        [Route("/v1/auth/currentuser")]
        [ProducesResponseType(typeof(CurrentUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CurrentUser()
        {
            Devon4NetLogger.Debug("Executing CurrentUser from controller AuthController");
            //Get claims
            var token = Request.Headers["Authorization"].ToString().Replace($"{AuthConst.AuthenticationScheme} ", string.Empty);

            if (string.IsNullOrEmpty(token)) return Unauthorized();

            var userClaims = JwtHandler.GetUserClaims(token).ToList();
            // Return result with claims values
            var result = new CurrentUserResponse
            {
                Id = JwtHandler.GetClaimValue(userClaims, ClaimTypes.NameIdentifier),
                UserName = JwtHandler.GetClaimValue(userClaims, ClaimTypes.Name),
                CorporateInfo = new List<CorporateBasicInfo> { new CorporateBasicInfo { Id = ClaimTypes.Role, Value = JwtHandler.GetClaimValue(userClaims, ClaimTypes.Role), } }
            };

            return Ok(result);
        }
    }
}