using Devon4Net.Domain.UnitOfWork.Service;
using Devon4Net.Domain.UnitOfWork.UnitOfWork;
using Devon4Net.Infrastructure.Logger.Logging;
using Devon4Net.Infrastructure.LiteDb.Repository;
using Devon4Net.Application.WebAPI.Implementation.Domain.Database;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Application.WebAPI.Implementation.Domain.RepositoryInterfaces;
using LiteDB;
namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    /// <summary>
    /// Session service implementation
    /// </summary>
    public class SessionService: ISessionService
    {
        //private readonly ISessionRepository _sessionRepository;
        private readonly ILiteDbRepository<Session> _sessionRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="SessionRepository"></param>
        public SessionService(ILiteDbRepository<Session> SessionRepository) 
        {
            _sessionRepository = SessionRepository;
        }

        /// <summary>
        /// Creates the Session
        /// </summary>
        /// <param name="expiresAt"></param>
        /// <param name="tasks"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        public async Task<BsonValue> CreateSession(DateTime expiresAt, IList<Domain.Entities.Task> tasks, IList<User> users)
        {
            Devon4NetLogger.Debug($"CreateSession method from service SessionService with value : {expiresAt}, {tasks}, {users}");

            //TODO: Exception Handling for DateTime expiresAt
            var newSession = new Session();
            newSession.ExpiresAt = expiresAt;
            newSession.Tasks = tasks;
            newSession.Users = users;
            return _sessionRepository.Create(newSession);
        }
    }
}