using System.ComponentModel.DataAnnotations;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Application.WebAPI.Implementation;
namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dto
{
    public class SessionDto
    {

        public long Id { get; set; }
        
        public string InviteToken { get; set; }

        [Required]
        public DateTime ExpiresAt  { get; set; }

        public IList<Domain.Entities.Task> Tasks  { get; set; }

        public IList<User> Users { get; set; }

        public Result? Result { get; set; }
    }
}


    