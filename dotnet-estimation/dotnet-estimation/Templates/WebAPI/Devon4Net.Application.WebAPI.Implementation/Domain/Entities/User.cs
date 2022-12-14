using LiteDB;

namespace Devon4Net.Application.WebAPI.Implementation.Domain.Entities
{
    public partial class User
    {
        [BsonId]
        public string Id { get; set; }

        public string Username { get; set; }

        public void Deconstruct(out string id, out string username)
        {
            id = Id;
            username = Username;
        }
    }
}