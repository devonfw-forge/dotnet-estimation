using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class StatusDto
    {
        public bool IsValid { get; set; }

        public string InviteToken { get; set; }

        public List<TaskDto> Tasks { get; set; }

        public List<UserDto> Users { get; set; }
    }
}