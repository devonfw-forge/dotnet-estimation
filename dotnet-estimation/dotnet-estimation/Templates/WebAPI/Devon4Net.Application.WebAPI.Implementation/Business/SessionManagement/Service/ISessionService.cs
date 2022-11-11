using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    /// <summary>
    /// TodoService interface
    /// </summary>
    public interface ISessionService
    {
        public Task<Session> GetSession(long id);

        public Task<(bool, Devon4Net.Application.WebAPI.Implementation.Domain.Entities.Task?)> GetStatus(long sessionId);

        public Task<bool> InvalidateSession(long sessionId);
        
        public Task<Estimation> AddNewEstimation(long sessionId , string VoteBy, int Conplexity);
    
        public Task<bool> RemoveUserFromSession(long id, String userId);
    }
}