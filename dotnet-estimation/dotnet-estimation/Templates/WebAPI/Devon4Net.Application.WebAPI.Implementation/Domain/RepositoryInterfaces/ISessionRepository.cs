using System.Linq.Expressions;
using Devon4Net.Domain.UnitOfWork.Repository;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;

namespace Devon4Net.Application.WebAPI.Implementation.Domain.RepositoryInterfaces
{
    /// <summary>
    /// SessionRepository interface
    /// </summary>
    public interface ISessionRepository : IRepository<Session>
    {   
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="expiresAt"></param>
        /// <param name="tasks"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        Task<Session> Create(DateTime expiresAt, IList<Domain.Entities.Task> tasks, IList<User> users);
    }
}
