using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos
{
    /// <summary>
    /// User definition
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class JoinSessionResultDto
    {
        public long SessionId { get; set; }

        public string Id { get; set; }

        public string Username { get; set; }

        public Role Role { get; set; }

        public string Token { get; set; }

        public bool Online { get; set; }

        public void Deconstruct(out long sessionId, out string userId, out string username, out Role role, out string token, out bool online)
        {
            sessionId = SessionId;
            userId = Id;
            username = Username;
            role = Role;
            token = Token;
            online = Online;
        }
    }
}
