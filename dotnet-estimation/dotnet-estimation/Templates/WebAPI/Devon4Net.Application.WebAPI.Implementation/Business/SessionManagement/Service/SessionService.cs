using Devon4Net.Domain.UnitOfWork.Service;
using Devon4Net.Domain.UnitOfWork.UnitOfWork;
using Devon4Net.Infrastructure.Logger.Logging;
using Devon4Net.Application.WebAPI.Implementation.Domain.Database;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Application.WebAPI.Implementation.Domain.RepositoryInterfaces;
namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    /// <summary>
    /// Session service implementation
    /// </summary>
    public class SessionService: Service<SessionContext>, ISessionService
    {
        private readonly ISessionRepository _sessionRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uoW"></param>
        public SessionService(IUnitOfWork<SessionContext> uoW) : base(uoW)
        {
            _sessionRepository = uoW.Repository<ISessionRepository>();
        }

        /// <summary>
        /// Creates the Session
        /// </summary>
        /// <param name="expiresAt"></param>
        /// <param name="tasks"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        public Task<Session> CreateSession(DateTime expiresAt, IList<Domain.Entities.Task> tasks, IList<User> users)
        {
            Devon4NetLogger.Debug($"CreateSession method from service SessionService with value : {expiresAt}, {tasks}, {users}");

            //TODO: Exception Handling for DateTime expiresAt

            return _sessionRepository.Create(expiresAt, tasks, users);
        }
    }
}