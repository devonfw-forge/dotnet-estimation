using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dto;
using LiteDB;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    /// <summary>
    /// ISessionService
    /// </summary>
    public interface ISessionService
    {
        public Task<Session> GetSession(long id);

        public Task<(bool, Devon4Net.Application.WebAPI.Implementation.Domain.Entities.Task?)> GetStatus(long sessionId);

        /// <summary>
        /// CreateSession
        /// </summary>
        /// <param name="sessionDto"></param>
        /// <returns></returns>
        public Task<BsonValue> CreateSession(SessionDto sessionDto);
    }
}