namespace Devon4Net.Application.WebAPI.Implementation.Domain.Entities
{
    public partial class Session
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