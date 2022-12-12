using System.Linq.Expressions;
using System.Collections.Generic;
using Devon4Net.Infrastructure.Logger.Logging;
using Devon4Net.Infrastructure.LiteDb.Repository;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Exceptions;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using System.Security.Cryptography;
using LiteDB;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Converters;
using ErrorOr;
using System;
using Devon4Net.Infrastructure.JWT.Handlers;
using System.Security.Claims;


namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    /// <summary>
    /// Session service implementation
    /// </summary>
    public class SessionService : ISessionService
    {
        private readonly ILiteDbRepository<Session> _sessionRepository;
        private readonly ILiteDbRepository<User> _userRepository;
        private readonly IJwtHandler _jwtHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="SessionRepository"></param>
        public SessionService(ILiteDbRepository<Session> SessionRepository, ILiteDbRepository<User> UserRepository, IJwtHandler JwtHandler)
        {
            _sessionRepository = SessionRepository;
            _userRepository = UserRepository;
            _jwtHandler = JwtHandler;
        }

        public ErrorOr<bool> validateSession(Session session, long sessionId)
        {

            if (session == null)
            {
                return Error.Failure(description: $"no session with the sessionId: {sessionId}");
            }

            if (!session.IsValid())
            {
                return Error.Failure(description: $"Session with the SessionId: {sessionId} is no longer valid");
            }
            return true;
        }

        /// <summary>
        /// Creates the Session
        /// </summary>
        /// <param name="sessionDto"></param>
        /// <returns></returns>
        public async Task<ErrorOr<ResultCreateSessionDto>> CreateSession(SessionDto sessionDto)

        {
            if (sessionDto.ExpiresAt <= DateTime.Now || sessionDto.ExpiresAt == null)
            {
                return Error.Failure(description: "Session is no longer valid or never existed");
            }

            var (expiresAt, username) = sessionDto;

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = username,
            };

            var userInsertResult = _userRepository.Create(user);

            var session = new Session
            {
                InviteToken = generateInviteToken(),
                ExpiresAt = sessionDto.ExpiresAt,
                Tasks = new List<Domain.Entities.Task>(),
                Users = new List<Domain.Entities.User>()
            };

            session.Users.Add(user);

            var result = _sessionRepository.Create(session);

            var (userUuid, _) = user;

            var userIdClaim = new Claim(ClaimTypes.NameIdentifier, userUuid);
            var userRoleClaim = new Claim(ClaimTypes.Role, "Admin");
            var userNameClaim = new Claim(ClaimTypes.Name, username);

            var token = _jwtHandler.CreateJwtToken(new List<Claim> {
                userRoleClaim,
                userNameClaim,
                userIdClaim,
            });

            return new ResultCreateSessionDto
            {
                Id = (long)result.RawValue,
                Token = token,
            };
        }

        public async Task<Session> GetSession(long id)
        {
            var expression = LiteDB.Query.EQ("_id", id);

            // FIXME: LiteDb also returs null values, when a matching entity does not exist!
            return _sessionRepository.GetFirstOrDefault(expression);
        }

        public async Task<ErrorOr<bool>> InvalidateSession(long sessionId)
        {
            Session session = await GetSession(sessionId);

            var ErrorOrTrue = validateSession(session, sessionId);

            if (ErrorOrTrue.IsError)
            {
                Devon4NetLogger.Debug(ErrorOrTrue.FirstError.Description);
                return ErrorOrTrue.FirstError;
            }

            session.ExpiresAt = DateTime.Now;

            return _sessionRepository.Update(session);
        }

        public async Task<ErrorOr<(bool, List<Domain.Entities.Task>, List<User>)>> GetStatus(long sessionId)
        {
            var session = await GetSession(sessionId);

            var ErrorOrTrue = validateSession(session, sessionId);

            if (ErrorOrTrue.IsError)
            {
                Devon4NetLogger.Debug(ErrorOrTrue.FirstError.Description);
                return ErrorOrTrue.FirstError;
            }

            return (true, session.Tasks.ToList(), entity.Users.ToList()));
        }

        /// <summary>
        /// Adds or updates a user's estimation to the active session's task
        /// </summary>
        /// <param name="sessionId">The current session's ID</param>
        /// <param name="taskId">The active task's ID</param>
        /// <param name="voteBy">The originating user's ID</param>
        /// <param name="complexity">Complexity estimation</param>
        /// <returns></returns>
        public async Task<ErrorOr<Estimation>> AddNewEstimation(long sessionId, string taskId, string voteBy, int complexity)
        {
            var session = await GetSession(sessionId);

            var ErrorOrTrue = validateSession(session, sessionId);

            if (ErrorOrTrue.IsError)
            {
                Devon4NetLogger.Debug(ErrorOrTrue.FirstError.Description);
                return ErrorOrTrue.FirstError;
            }
            var containsTask = session.Tasks.Where(item => item.Id == taskId).Any();

            if (containsTask == false)
            {
                return Error.Failure(description: $"Session doesn't contain Task with TaskId : {taskId}");
            }

            var task = session.Tasks.First(item => item.Id == taskId);

            var estimations = task.Estimations;

            var newEstimation = new Estimation { VoteBy = voteBy, Complexity = complexity };

            // if estimation by user already exists, delete previous estimation before adding new
            if (estimations != null && estimations.Any())
            {
                var alreadyContainsEstimation = estimations.Where(item => item.VoteBy == voteBy).Any();

                if (alreadyContainsEstimation)
                {
                    var oldEstimation = estimations.FirstOrDefault(est => est.VoteBy == voteBy);


                    estimations.Remove(oldEstimation);
                }
            }

            estimations.Add(newEstimation);

            _sessionRepository.Update(session);

            return newEstimation;
        }

        public async Task<ErrorOr<bool>> RemoveUserFromSession(long sessionId, string userId)
        {
            var expression = LiteDB.Query.EQ("_id", sessionId);
            var session = _sessionRepository.GetFirstOrDefault(expression);

            var ErrorOrTrue = validateSession(session, sessionId);

            if (ErrorOrTrue.IsError)
            {
                Devon4NetLogger.Debug(ErrorOrTrue.FirstError.Description);
                return ErrorOrTrue.FirstError;
            }

            var user = session.Users.SingleOrDefault(i => i.Id == userId);

            if (user != null)
            {
                session.Users.Remove(user);
                _sessionRepository.Update(session);
                return true;
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

        public async Task<ErrorOr<(bool, UserDto?)>> AddUserToSession(long sessionId, string username)
        {
            var session = await GetSession(sessionId);


            var newUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = username,
            };

            var userEntity = _userRepository.Create(newUser);

            bool finished = false;
            
            var ErrorOrTrue = validateSession(session, sessionId);
            
            if (ErrorOrTrue.IsError)
            {
                Devon4NetLogger.Debug(ErrorOrTrue.FirstError.Description);
                return ErrorOrTrue.FirstError;
            }
            
            session.Users.Add(newUser);

            finished = _sessionRepository.Update(session);

            Devon4NetLogger.Debug("Added user to session!");
            

            if (finished)
            {
                var (userUuid, _) = newUser;

                var userIdClaim = new Claim(ClaimTypes.NameIdentifier, userUuid);
                var userRoleClaim = new Claim(ClaimTypes.Role, "Voter");
                var userNameClaim = new Claim(ClaimTypes.Name, username);

                var token = _jwtHandler.CreateJwtToken(new List<Claim> {
                    userRoleClaim,
                    userNameClaim,
                    userIdClaim
                });

                Devon4NetLogger.Debug("Returned token: " + token);

                var resultingUser = new UserDto
                {
                    Id = userUuid,
                    Username = username,
                    Token = token
                };

                return (true, resultingUser);
            }
        }
        
        
        public async Task<ErrorOr<(bool, TaskDto)>> AddTaskToSession(long sessionId, string userId, TaskDto task)

        {
            var session = await GetSession(sessionId);

            if (!session.isPrivilegedUser(userId))
            {
                return (false, null);
            }

            var (title, description, url, status) = task;

            var newTask = new Domain.Entities.Task
            {
                Id = Guid.NewGuid().ToString(),
                Title = title,
                Description = description,
                Estimations = new List<Estimation>(),
                Url = url,
                Status = Status.Suspended
            };


            var ErrorOrTrue = validateSession(session, sessionId);

            if (ErrorOrTrue.IsError)
            {
                Devon4NetLogger.Debug(ErrorOrTrue.FirstError.Description);
                return ErrorOrTrue.FirstError;
            }
            session.Tasks.Add(newTask);

            var finished = _sessionRepository.Update(session);

            if (finished)
            {
                return (finished, TaskConverter.ModelToDto(newTask));
            }
            
            return (false, null);
        }

        /// <summary>
        /// Delete a Task
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<ErrorOr<bool>> DeleteTask(long sessionId, string taskId)
        {
            var session = await GetSession(sessionId);

            var ErrorOrTrue = validateSession(session, sessionId);

            if (ErrorOrTrue.IsError)
            {
                Devon4NetLogger.Debug(ErrorOrTrue.FirstError.Description);
                return ErrorOrTrue.FirstError;
            }

            var task = session.Tasks.ToList().Find(item => item.Id == taskId);

            if (task == null)
            {
                return Error.Failure(description: $"Session doesn't contain Task with TaskId : {taskId}");
            }

            session.Tasks.Remove(task);

            return _sessionRepository.Update(session);
        }

        private string generateInviteToken()
        {   //generates 8 random bytes and returns them as a token string 
            byte[] randomNumber = new byte[8];
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            rngCsp.GetNonZeroBytes(randomNumber);
            return BitConverter.ToString(randomNumber);
        }

        public async Task<ErrorOr<(bool, List<TaskStatusChangeDto>)>> ChangeTaskStatus(long sessionId, TaskStatusChangeDto statusChange)
        {
            var session = await GetSession(sessionId);

            var (id, status) = statusChange;

            var ErrorOrTrue = validateSession(session, sessionId);

            if (ErrorOrTrue.IsError)
            {
                Devon4NetLogger.Debug(ErrorOrTrue.FirstError.Description);
                return ErrorOrTrue.FirstError;
            }

            var containsTask = session.Tasks.Where(item => item.Id == id).Any();

            if (!containsTask)
            {
                return (false, new List<TaskStatusChangeDto>());
            }

            // and it contains the task requested to be chnaged
            var (modified, taskChanges) = session.ChangeStatusOfTask(id, status);

            if (!modified)
            {
                return (false, new List<TaskStatusChangeDto>());
            }

            var finished = _sessionRepository.Update(session);

            // and we could properly update the database
            if (finished)
            {
                var converted = taskChanges.Select<(String id, Status status), TaskStatusChangeDto>(item => new TaskStatusChangeDto { Id = item.id, Status = item.status }).ToList();

                return (true, converted);
            }
            

            return (false, new List<TaskStatusChangeDto>());
        }

        public async Task<bool> isPrivilegedUser(long sessionId, string userId)
        {
            var session = await GetSession(sessionId);

            if (session is null || !session.IsValid())
            {
                return false;
            }

            if (session.Users.First().Id.Equals(userId))
            {
                return true;
            }

            return false;
        }
    }
}
