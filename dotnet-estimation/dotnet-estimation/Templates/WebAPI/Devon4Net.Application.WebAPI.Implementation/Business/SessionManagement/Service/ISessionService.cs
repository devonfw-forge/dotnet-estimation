using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
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
        /// <param name="expiresAt"></param>
        /// <param name="tasks"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        Task<BsonValue> CreateSession(DateTime expiresAt, IList<Domain.Entities.Task> tasks, IList<User> users);
    }
}