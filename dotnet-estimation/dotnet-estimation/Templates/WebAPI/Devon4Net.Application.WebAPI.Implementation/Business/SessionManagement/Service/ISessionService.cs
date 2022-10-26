using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    /// <summary>
    /// TodoService interface
    /// </summary>
    public interface ISessionService
    {
        /// <summary>
        /// Get Session with the given id
        /// </summary>
        public Task<Session> GetSession(long id);

        /// <summary>
        /// Add an User to a given session
        /// </summary>
        public Task<bool> AddUserToSession(long sessionId, string userId, Role role);

        /// <summary>
        /// Get users of a session
        /// </summary>
        public Task<IList<User>> GetSessionUsers(long id);
    }
}