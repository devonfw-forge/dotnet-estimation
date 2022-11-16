using Devon4Net.Application.WebAPI.Implementation.Business.AuthManagement.Dto;

namespace Devon4Net.Application.WebAPI.Implementation.Business.AuthManagement.Service
{
    public interface IAuthService
    {
        /// <summary>
        /// Generate an AccessToken for a given username
        /// </summary>
        /// <param name="username">Session username</param>
        /// <returns></returns>
        Task<LoginResponse> getTokenUsername(string username);

        Task<LoginResponse> login(string user, string password);
    }
}
