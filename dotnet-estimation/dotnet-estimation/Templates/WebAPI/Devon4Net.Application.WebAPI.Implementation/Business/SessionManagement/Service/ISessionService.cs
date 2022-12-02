using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;

using LiteDB;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    /// <summary>
    /// ISessionService
    /// </summary>
    public interface ISessionService
    {
        public Task<Session> GetSession(long id);

        public Task<(bool, List<Devon4Net.Application.WebAPI.Implementation.Domain.Entities.Task>)> GetStatus(long sessionId);

        /// <summary>
        /// CreateSession
        /// </summary>
        /// <param name="sessionDto"></param>
        /// <returns></returns>
        public Task<ResultCreateSessionDto> CreateSession(SessionDto sessionDto);
        public Task<bool> InvalidateSession(long sessionId);

        public Task<Estimation> AddNewEstimation(long sessionId, string taskId, string voteBy, int complexity);

        public Task<bool> RemoveUserFromSession(long id, String userId);

        /// <summary>
        /// Add an User to a given session
        /// </summary>
        public Task<bool> AddUserToSession(long sessionId, string userId, Role role);
        public Task<(bool, TaskDto?)> AddTaskToSession(long sessionId, TaskDto task);

        /// <summary>
        /// Delete a Task
        /// </summary>
        public Task<bool> DeleteTask(long sessionId, string taskId);

        public Task<(bool, List<TaskStatusChangeDto>)> ChangeTaskStatus(long sessionId, TaskStatusChangeDto statusChange);
    }
}