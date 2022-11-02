<<<<<<< HEAD
﻿using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
=======
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
>>>>>>> 41c3a71fc865b7a5013a56dc38674e0f84353060

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    /// <summary>
    /// TodoService interface
    /// </summary>
    public interface ISessionService
    {
<<<<<<< HEAD
        /// <summary>
        /// Get Session with the given id
        /// </summary>
        public Task<Session> GetSession(long id);

        /// <summary>
        /// Add an User to a given session
        /// </summary>
        public Task<bool> AddUserToSession(long sessionId, string userId, Role role);

        /// <summary>
        /// Get users of a session
        /// </summary>
        public Task<IList<User>> GetSessionUsers(long id);
=======
        public Task<Session> GetSession(long id);

        public Task<(bool, Devon4Net.Application.WebAPI.Implementation.Domain.Entities.Task?)> GetStatus(long sessionId);
>>>>>>> 41c3a71fc865b7a5013a56dc38674e0f84353060
    }
}