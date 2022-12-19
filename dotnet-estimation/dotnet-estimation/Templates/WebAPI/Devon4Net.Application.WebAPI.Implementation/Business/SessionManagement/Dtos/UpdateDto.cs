using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]

    public class UpdateDto
    {
        public List<string> AvailableClients { get; set; }
    }
}
