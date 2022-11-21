using System.Linq.Expressions;
using System.Collections.Generic;
using Devon4Net.Infrastructure.Logger.Logging;
using Devon4Net.Infrastructure.LiteDb.Repository;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Exceptions;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using System.Security.Cryptography;
using LiteDB;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    /// <summary>
    /// Session service implementation
    /// </summary>
    public class SessionService : ISessionService
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
            if (sessionDto.ExpiresAt <= DateTime.Now || sessionDto.ExpiresAt == null)
            {
                throw new InvalidExpiryDateException();
            }

            return _sessionRepository.Create(new Session
            {
                InviteToken = generateInviteToken(),
                ExpiresAt = sessionDto.ExpiresAt,
                Tasks = new List<Domain.Entities.Task>(),
                Users = new List<Domain.Entities.User>()
            });
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
        public async Task<Estimation> AddNewEstimation(long sessionId, string voteBy, int complexity)
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

            var newEstimation = new Estimation { VoteBy = voteBy, Complexity = complexity };

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
        public async Task<bool> RemoveUserFromSession(long sessionId, string userId)
        {
            var expression = LiteDB.Query.EQ("_id", sessionId);
            var session = _sessionRepository.GetFirstOrDefault(expression);

            if (session != null)
            {
                var user = session.Users.SingleOrDefault(i => i.Id == userId);

                if (user != null)
                {
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
            var session = await GetSession(sessionId);

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
                Console.WriteLine("Session not found!");

                if (!session.Tasks.Any(x => x.Equals(newTask)))
                {
                    session.Tasks.Add(newTask);

                    return _sessionRepository.Update(session);
                }
            }
            return false;
        }

        private string generateInviteToken()
        {   //generates 8 random bytes and returns them as a token string 
            byte[] randomNumber = new byte[8];
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            rngCsp.GetNonZeroBytes(randomNumber);
            return BitConverter.ToString(randomNumber);
        }

        public async Task<(bool, List<TaskStatusChangeDto>)> ChangeTaskStatus(long sessionId, TaskStatusChangeDto statusChange)
        {
            var session = await GetSession(sessionId);

            var (id, status) = statusChange;

            if (session is not null)
            {
                var containsTask = session.Tasks.Where(item => item.Id == id).Any();

                if (!containsTask)
                {
                    return (false, new List<TaskStatusChangeDto>());
                }

                var (modified, taskChanges) = session.ChangeStatusOfTask(id, status);

                var finished = _sessionRepository.Update(session);

                if (modified && finished)
                {
                    var converted = taskChanges.Select<(String id, Status status), TaskStatusChangeDto>(item => new TaskStatusChangeDto { Id = item.id, Status = item.status }).ToList();

                    return (true, converted);
                }
            }

            return (false, new List<TaskStatusChangeDto>());
        }
    }
}