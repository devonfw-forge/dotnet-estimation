using System.Linq.Expressions;
using Devon4Net.Infrastructure.LiteDb.Repository;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dto;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    /// <summary>
    /// Session service implementation
    /// </summary>
    public class SessionService : ISessionService
    {
        private readonly ILiteDbRepository<Session> _sessionRepository;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="SessionRepository"></param>
        public SessionService(ILiteDbRepository<Session> SessionRepository)
        {
            _sessionRepository = SessionRepository;
        }

        /// <summary>
        /// Get the session for the given id
        /// </summary>
        /// <param name="id">Id of the searched Session</param>
        /// <returns></returns>
        public async Task<Session> GetSession(long id)
        {
            var expression = LiteDB.Query.EQ("_id", id);

            return _sessionRepository.GetFirstOrDefault(expression);
        }

        /// <summary>
        /// Add an user to a given session
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<bool> AddUserToSession(long sessionId, string userId, Role role)
        {
            var expression = LiteDB.Query.EQ("_id", sessionId);
            var session = _sessionRepository.GetFirstOrDefault(expression);
            var newUser = new User {
                Id = userId,
                Role = role,
            };

            if(session != null)
            {
                if(!session.Users.Any(x => x.Equals(newUser)))
                {
                    session.Users.Add(newUser);
                    return _sessionRepository.Update(session);
                }
            }
            return false;
        }

        public async Task<IList<User>> GetSessionUsers(long id)
        {
            var expression = LiteDB.Query.EQ("_id", id);
            var session = _sessionRepository.GetFirstOrDefault(expression);

            if (session != null)
            {
                return session.Users;
            }

            IList<User> noUsers = new User[0];
            return noUsers;
        }
    }
}