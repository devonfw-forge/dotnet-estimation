using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class AuthenticatedUserDto
    {
        public string Id { get; set; }
        public string Username { get; set; }

        public Role Role { get; set; }

        public void Deconstruct(out string id, out string username)
        {
            id = Id;
            username = Username;
        }
    }
}