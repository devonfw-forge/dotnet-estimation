namespace Devon4Net.Application.WebAPI.Implementation.Domain.Entities
{
    public class Session 
    {
        public long Id { get; set; }
        
        public string InviteToken { get; set; }

        public DateTime ExpiresAt  { get; set; }

        public IList<Task> Tasks  { get; set; }

        public IList<User> Users { get; set; }

        public Result? Result { get; set; }
    }
}