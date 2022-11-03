using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dto;
using LiteDB;
namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    /// <summary>
    /// ISessionService
    /// </summary>
    public interface ISessionService
    {
        /// <summary>
        /// CreateSession
        /// </summary>
        /// <param name="sessionDto"></param>
        /// <returns></returns>
        Task<BsonValue> CreateSession(SessionDto sessionDto);
    }
}