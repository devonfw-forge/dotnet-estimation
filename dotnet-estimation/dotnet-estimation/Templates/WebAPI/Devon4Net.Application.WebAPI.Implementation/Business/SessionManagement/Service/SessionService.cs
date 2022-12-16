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
        /// <summary>
        /// Creates the Session
        /// </summary>
        /// <param name="sessionDto"></param>
        /// <returns></returns>
        public async Task<ResultCreateSessionDto> CreateSession(SessionDto sessionDto)
        {
            if (sessionDto.ExpiresAt <= DateTime.Now || sessionDto.ExpiresAt == null)
            {
                throw new InvalidExpiryDateException();
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

        public async Task<Session> FindSessionWithInviteToken(string token)
        {
            var expression = LiteDB.Query.EQ("InviteToken", token);

            return _sessionRepository.GetFirstOrDefault(expression);
        }

        public async Task<Session> GetSession(long id)
        {
            var expression = LiteDB.Query.EQ("_id", id);

            // FIXME: LiteDb also returs null values, when a matching entity does not exist!
            return _sessionRepository.GetFirstOrDefault(expression);
        }

        public async Task<Session> GetSessionAndUsers(long id)
        {
            var session = await GetSession(id);
            var usersResult = new List<Domain.Entities.User>();

            foreach (var user in session.Users)
            {
                var expression = LiteDB.Query.EQ("_id", user.Id);
                var dbUser = _userRepository.GetFirstOrDefault(expression);
                usersResult.Add(dbUser);
            }
            session.Users = usersResult;
            return session;
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

        public async Task<(bool, string?, List<Domain.Entities.Task>, List<User>)> GetStatus(long sessionId)
        {
            var entity = await GetSessionAndUsers(sessionId);
            if (entity == null)
            {
                throw new NotFoundException(sessionId);
            }

            bool sessionIsValid = entity.IsValid();

            if (!sessionIsValid)
            {
                return (false, null, null, null);
            }

            return (true, entity.InviteToken, entity.Tasks.ToList(), entity.Users.ToList());
        }

        /// <summary>
        /// Adds or updates a user's estimation to the active session's task
        /// </summary>
        /// <param name="sessionId">The current session's ID</param>
        /// <param name="taskId">The active task's ID</param>
        /// <param name="voteBy">The originating user's ID</param>
        /// <param name="complexity">Complexity estimation</param>
        /// <returns></returns>
        public async Task<Estimation> AddNewEstimation(long sessionId, string taskId, string voteBy, int complexity)
        {
            var session = await GetSession(sessionId);

            if (session is null)
            {
                throw new NoLongerValid(sessionId);
            }

            var containsTask = session.Tasks.Where(item => item.Id == taskId).Any();

            if (containsTask == false)
            {
                throw new NoOpenOrSuspendedTask();
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
                    var oldEstimation = estimations.First(est => est.VoteBy == voteBy);

                    estimations.Remove(oldEstimation);
                }
            }

            estimations.Add(newEstimation);

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
        public async Task<(bool, JoinSessionResultDto?)> AddUserToSession(string inviteToken, string username, Role desiredRole)
        {
            if (desiredRole == Role.Author)
            {
                return (false, null);
            }

            var session = await FindSessionWithInviteToken(inviteToken);

            if (!session.IsValid())
            {
                Devon4NetLogger.Debug("Session is not valid!");
            }

            Devon4NetLogger.Debug("Adding to session!");

            var newUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = username,
            };

            var userEntity = _userRepository.Create(newUser);

            bool finished = false;

            if (session is not null)
            {
                session.Users.Add(newUser);

                finished = _sessionRepository.Update(session);

                Devon4NetLogger.Debug("Added user to session!");
            };

            if (finished)
            {
                var (userUuid, _) = newUser;

                var userIdClaim = new Claim(ClaimTypes.NameIdentifier, userUuid);
                var userRoleClaim = new Claim(ClaimTypes.Role, desiredRole.ToString());
                var userNameClaim = new Claim(ClaimTypes.Name, username);

                var token = _jwtHandler.CreateJwtToken(new List<Claim> {
                    userRoleClaim,
                    userNameClaim,
                    userIdClaim
                });

                Devon4NetLogger.Debug("Returned token: " + token);

                var joinResult = new JoinSessionResultDto
                {
                    SessionId = session.Id,
                    Id = userUuid,
                    Username = username,
                    Token = token,
                    Role = desiredRole
                };

                return (true, joinResult);
            }

            return (false, null);
        }

        public async Task<(bool, TaskDto)> AddTaskToSession(long sessionId, string userId, TaskDto task)
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

            if (session != null)
            {
                session.Tasks.Add(newTask);

                var finished = _sessionRepository.Update(session);

                if (finished)
                {
                    return (finished, TaskConverter.ModelToDto(newTask));
                }
            }

            return (false, null);
        }

        /// <summary>
        /// Delete a Task
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteTask(long sessionId, string taskId)
        {
            var session = await GetSession(sessionId);

            if (session == null)
            {
                throw new NotFoundException(sessionId);
            }

            var task = session.Tasks.ToList().Find(item => item.Id == taskId);

            if (task == null)
            {
                throw new TaskNotFoundException(taskId);
            }

            session.Tasks.Remove(task);

            return _sessionRepository.Update(session);
        }

        private string generateInviteToken()
        {   //generates 8 random bytes and returns them as a token string 
            byte[] randomNumber = new byte[8];
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            rngCsp.GetNonZeroBytes(randomNumber);
            return BitConverter.ToString(randomNumber).Replace("-", String.Empty);
        }

        public async Task<(bool, List<TaskStatusChangeDto>)> ChangeTaskStatus(long sessionId, TaskStatusChangeDto statusChange)
        {
            var session = await GetSession(sessionId);

            var (id, status) = statusChange;

            // if the session could be found and is valid
            if (session is not null && session.IsValid())
            {
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
