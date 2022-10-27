using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
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