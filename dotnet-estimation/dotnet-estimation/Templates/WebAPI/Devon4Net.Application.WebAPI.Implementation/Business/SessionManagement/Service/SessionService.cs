using System.Linq.Expressions;
using System.Collections.Generic;
using Devon4Net.Infrastructure.LiteDb.Repository;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Exceptions;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    public class SessionService: ISessionService
    {
        private readonly ILiteDbRepository<Session> _sessionRepository;

        public SessionService(ILiteDbRepository<Session> SessionRepository) 
        {
            _sessionRepository = SessionRepository;
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

        public async Task<Estimation> AddNewEstimation(long sessionId, string VoteBy, int Complexity)
        {
           var (isvalid, currentTask) = await GetStatus(sessionId);
           if (!isvalid)
            {
                throw new NoLongerValid(sessionId);
            }
           if (currentTask == null)
            {
                throw new NoOpenOrSuspendedTask();
            }

           var  newEstimation = new Estimation { VoteBy = VoteBy, Complexity = Complexity};

            var session = await GetSession(sessionId);

            for (int i = 0; i < session.Tasks.Count; i++)
            {
                if (session.Tasks[i].Id == currentTask.Id)
                {
                    session.Tasks[i].Estimations.Add(newEstimation);
                }
            }

            _sessionRepository.Update(session);

            return newEstimation;
        }
        
        /// <summary>
        /// ARemove a specified user from a given session
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveUserFromSession(long sessionId, string userId)
        {
            var expression = LiteDB.Query.EQ("_id", sessionId);
            var session = _sessionRepository.GetFirstOrDefault(expression);

            if(session != null) {
                var user = session.Users.SingleOrDefault(i => i.Id == userId);

                if (user != null) {
                    session.Users.Remove(user);
                    _sessionRepository.Update(session);
                    return true;
                }
            }

            return false;
        }
    }
}