using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    /// <summary>
    /// TodoService interface
    /// </summary>
    public interface ISessionService
    {
        public Task<Session> GetSession(long id);

        public Task<(bool, Devon4Net.Application.WebAPI.Implementation.Domain.Entities.Task?)> GetStatus(long sessionId);

        /// <summary>
        /// Add an User to a given session
        /// </summary>
        public Task<bool> AddUserToSession(long sessionId, string userId, Role role);
    }
}