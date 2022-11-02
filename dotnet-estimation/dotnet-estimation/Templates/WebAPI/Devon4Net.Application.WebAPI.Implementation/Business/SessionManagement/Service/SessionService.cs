using System.Linq.Expressions;
using Devon4Net.Infrastructure.LiteDb.Repository;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dto;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Exceptions;
using System.Collections.Generic;

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

        public async Task<(bool, Domain.Entities.Task?)> GetStatus(long sessionId) {
            var sessionResult = await GetSession(sessionId);
            
            if (sessionResult == null)
            {
                throw new NotFoundException(sessionId);
            }

            bool sessionIsValid = sessionResult.isValid();
            
            if (!sessionIsValid)
            {
                return (false, null);
            }

            // since there can be only one task which is being evaluated,
            // we only query the first object
            var evaluatedTask = sessionResult.Tasks.ToList().Find(item => item.Status == Status.Evaluated);
            
            if (evaluatedTask is not null) {
                return (sessionIsValid, evaluatedTask);
            }

            // else we try to find tasks which are open
            var openTasks = sessionResult.Tasks.Where(item => item.Status == Status.Open).ToList();

            if (openTasks.Any()) 
            {
                openTasks.Sort((x, y) => DateTime.Compare(x.CreatedAt, y.CreatedAt));

                var currentTask = openTasks.First();

                return (sessionIsValid, currentTask);
            } 
            else
            {
                // if there are no open tasks left to be estimated we query for postponed tasks
                var suspendedTasks = sessionResult.Tasks.Where(item => item.Status == Status.Suspended).ToList();

                suspendedTasks.Sort((x, y) => DateTime.Compare(x.CreatedAt, y.CreatedAt));

                if (suspendedTasks.Any())
                {
                    var currentTask = suspendedTasks.First();

                    return (sessionIsValid, currentTask);
                }
            }

            return (sessionIsValid, null);
        }
    }
}