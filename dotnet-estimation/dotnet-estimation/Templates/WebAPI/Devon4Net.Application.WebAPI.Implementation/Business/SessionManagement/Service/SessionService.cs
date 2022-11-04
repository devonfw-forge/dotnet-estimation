using System.Linq.Expressions;
using System.Collections.Generic;
using Devon4Net.Infrastructure.Logger.Logging;
using Devon4Net.Infrastructure.LiteDb.Repository;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Exceptions;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dto;
using System.Security.Cryptography;
using LiteDB;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    /// <summary>
    /// Session service implementation
    /// </summary>
    public class SessionService: ISessionService
    {
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
        /// <param name="sessionDto"></param>
        /// <returns></returns>
        public async Task<BsonValue> CreateSession(SessionDto sessionDto)
        {
            Devon4NetLogger.Debug($"CreateSession method from service SessionService with value : {sessionDto.ExpiresAt}");

            return _sessionRepository.Create(new Session{
                InviteToken = generateToken(),
                ExpiresAt = sessionDto.ExpiresAt
            });
        }
        
        public async Task<Session> GetSession(long id)
        {
            var expression = LiteDB.Query.EQ("_id", id);

            // FIXME: LiteDb also returs null values, when a matching entity does not exist!
            return _sessionRepository.GetFirstOrDefault(expression);
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

        private string generateToken()
        {   //generates 8 random bytes and returns them as a token string 
            byte[] randomNumber = new byte[8];
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            rngCsp.GetNonZeroBytes(randomNumber);
            return BitConverter.ToString(randomNumber);
        }
    }
}