using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dto
{
    /// <summary>
    /// Session definition
    /// </summary>
    public class SessionDto
    {
        /// <summary>
        /// the Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// the InviteToken
        /// </summary>
        [Required]
        public string InviteToken { get; set; }

        /// <summary>
        /// the expiration time
        /// </summary>
        [Required]
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// the Tasks
        /// </summary>
        [Required]
        public IList<Domain.Entities.Task> Tasks { get; set; }

        /// <summary>
        /// the Users
        /// </summary>
        public IList<User> Users { get; set; }

        /// <summary>
        /// the Result
        /// </summary>
        public Result Result { get; set; }
    }
}
