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
    public class JoinSessionDto
    {
        public string Username { get; set; }

        public Role Role { get; set; }
        public void Deconstruct(out string username, out Role role)
        {
            username = Username;
            role = Role;
        }
    }
}
