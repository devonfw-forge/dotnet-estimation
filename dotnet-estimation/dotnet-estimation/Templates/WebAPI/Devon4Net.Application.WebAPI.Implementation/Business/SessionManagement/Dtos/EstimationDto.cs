using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class EstimationDto
    {
        public string TaskId { get; set; }

        public string VoteBy { get; set; }

        public int Complexity { get; set; }

        public void Deconstruct(out string taskId, out string voteBy, out int complexity)
        {
            taskId = TaskId;
            voteBy = VoteBy;
            complexity = Complexity;
        }
    }
}
