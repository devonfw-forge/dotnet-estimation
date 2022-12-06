using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos
{
    /// <summary>
    /// User definition
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// the Id
        /// </summary>
        public string Id { get; set; }
        public string Username { get; set; }
    }
}
