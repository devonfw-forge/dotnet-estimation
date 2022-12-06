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

        public Task<ErrorOr<(bool, List<Domain.Entities.Task>)>> GetStatus(long sessionId);

        /// <summary>
        /// CreateSession
        /// </summary>
        /// <param name="sessionDto"></param>
        /// <returns></returns>
        public Task<ErrorOr<BsonValue>> CreateSession(SessionDto sessionDto);
        public Task<ErrorOr<bool>> InvalidateSession(long sessionId);

        public Task<ErrorOr<Estimation>> AddNewEstimation(long sessionId, string taskId, string voteBy, int complexity);

        public Task<ErrorOr<bool>> RemoveUserFromSession(long id, String userId);

        /// <summary>
        /// Add an User to a given session
        /// </summary>
        public Task<ErrorOr<bool>> AddUserToSession(long sessionId, string userId, Role role);
        public Task<ErrorOr<(bool, TaskDto?)>> AddTaskToSession(long sessionId, TaskDto task);

        /// <summary>
        /// Delete a Task
        /// </summary>
        public Task<ErrorOr<bool>> DeleteTask(long sessionId, string taskId);

        public Task<ErrorOr<(bool, List<TaskStatusChangeDto>)>> ChangeTaskStatus(long sessionId, TaskStatusChangeDto statusChange);
    }
}
