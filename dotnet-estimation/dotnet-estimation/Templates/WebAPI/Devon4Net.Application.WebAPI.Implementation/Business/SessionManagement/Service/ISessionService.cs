using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using ErrorOr;
using LiteDB;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    /// <summary>
    /// ISessionService
    /// </summary>
    public interface ISessionService
    {
        public Task<Session> GetSession(long id);

        public Task<ErrorOr(bool, List<Devon4Net.Application.WebAPI.Implementation.Domain.Entities.Task>, List<User>)>> GetStatus(long sessionId)

        /// <summary>
        /// CreateSession
        /// </summary>
        /// <param name="sessionDto"></param>
        /// <returns></returns>

        public Task<ErrorOr<ResultCreateSessionDto>> CreateSession(SessionDto sessionDto);
        public Task<ErrorOr<bool>> InvalidateSession(long sessionId);

        public Task<ErrorOr<Estimation>> AddNewEstimation(long sessionId, string taskId, string voteBy, int complexity);

        public Task<ErrorOr<bool>> RemoveUserFromSession(long id, String userId);

        /// <summary>
        /// Add an User to a given session
        /// </summary>
        public Task<ErrorOr<(bool, UserDto?)>> AddUserToSession(long sessionId, string username);
        public Task<ErrorOr<(bool, TaskDto?)>> AddTaskToSession(long sessionId, string userId, TaskDto task);

        /// <summary>
        /// Delete a Task
        /// </summary>
        public Task<ErrorOr<bool>> DeleteTask(long sessionId, string taskId);

        public Task<ErrorOr<(bool, List<TaskStatusChangeDto>)>> ChangeTaskStatus(long sessionId, TaskStatusChangeDto statusChange);

        public Task<bool> isPrivilegedUser(long sessionId, string userId);
    }
}
