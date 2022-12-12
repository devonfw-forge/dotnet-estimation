using System.ComponentModel.DataAnnotations;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Application.WebAPI.Implementation;
namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos
{
    /// <summary>
    /// Session definition
    /// </summary>
    public class SessionDto
    {
        /// <summary>
        /// the Expiry Date
        /// </summary>
        [Required]
        public DateTime ExpiresAt { get; set; }

        [Required]
        public String Username { get; set; }

        public void Deconstruct(out DateTime expiresAt, out string username)
        {
            expiresAt = ExpiresAt;
            username = Username;
        }
    }
}
