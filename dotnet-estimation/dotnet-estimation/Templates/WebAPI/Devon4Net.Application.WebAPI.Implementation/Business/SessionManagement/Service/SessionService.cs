using Devon4Net.Infrastructure.LiteDb.Repository;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Exceptions;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    public class SessionService : ISessionService
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

        public async Task<bool> InvalidateSession(long sessionId)
        {
            Session sessionResult = await GetSession(sessionId);

            if (sessionResult == null)
            {
                throw new NotFoundException(sessionId);
            }

            if (!sessionResult.IsValid())
            {
                throw new InvalidSessionException(sessionId);
            }

            sessionResult.ExpiresAt = DateTime.Now;

            return _sessionRepository.Update(sessionResult);
        }

        public async Task<(bool, Domain.Entities.Task?)> GetStatus(long sessionId)
        {
            var sessionResult = await GetSession(sessionId);

            if (sessionResult == null)
            {
                throw new NotFoundException(sessionId);
            }

            bool sessionIsValid = sessionResult.IsValid();

            if (!sessionIsValid)
            {
                return (false, null);
            }

            // since there can be only one task which is being evaluated,
            // we only query the first object
            var evaluatedTask = sessionResult.Tasks.ToList().Find(item => item.Status == Status.Evaluated);

            if (evaluatedTask is not null)
            {
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

            var newEstimation = new Estimation { VoteBy = VoteBy, Complexity = Complexity };

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
            var newUser = new User
            {
                Id = userId,
                Role = role,
            };

            if (session != null)
            {
                if (!session.Users.Any(x => x.Equals(newUser)))
                {
                    session.Users.Add(newUser);
                    return _sessionRepository.Update(session);
                }
            }
            return false;
        }

        public async Task<bool> AddTaskToSession(long sessionId, TaskDto task)
        {
           /* var newSession = new Session
            {
                Id = 1,
                InviteToken = "asdf",
                ExpiresAt = DateTime.Now.AddMinutes(25),
                Tasks = new List<Domain.Entities.Task>(),
                Users = new List<Domain.Entities.User>(),
            };
            _sessionRepository.Create(newSession);
           */
            var expression = LiteDB.Query.EQ("_id", sessionId);
            var session = _sessionRepository.GetFirstOrDefault(expression);
            var (id, title, description, url, status) = task;

            var newTask = new Domain.Entities.Task
            {
                Id = id,
                Title = title,
                Description = description,
                Url = url,
                Status = status
            };

            if (session != null)
            {
                if (!session.Tasks.Any(x => x.Equals(newTask)))
                {
                    session.Tasks.Add(newTask);



                    return _sessionRepository.Update(session);
                }
            }
            return false;
        }
    }
}