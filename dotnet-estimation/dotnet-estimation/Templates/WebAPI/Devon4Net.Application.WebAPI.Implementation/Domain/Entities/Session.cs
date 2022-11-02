namespace Devon4Net.Application.WebAPI.Implementation.Domain.Entities
{
<<<<<<< HEAD
    public class Session 
=======
    public partial class Session
>>>>>>> 41c3a71fc865b7a5013a56dc38674e0f84353060
    {
        public long Id { get; set; }

        public string InviteToken { get; set; }

        public DateTime ExpiresAt { get; set; }

        public IList<Task> Tasks { get; set; }

        public IList<User> Users { get; set; }

        public bool isValid() {
            return ExpiresAt > DateTime.Now;
        }
    }
}