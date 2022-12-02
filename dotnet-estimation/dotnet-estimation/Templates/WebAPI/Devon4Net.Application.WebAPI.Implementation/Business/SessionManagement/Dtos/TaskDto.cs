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
    public class TaskDto
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public string? Url { get; set; }

        public Status Status { get; set; }

        public List<EstimationDto> Estimations { get; set; }

        public void Deconstruct(out string id, out string title, out string? description, out string? url, out Status status)
        {
            id = Id;
            title = Title;
            description = Description;
            url = Url;
            status = Status;
        }

        public void Deconstruct(out string title, out string? description, out string? url, out Status status)
        {
            title = Title;
            description = Description;
            url = Url;
            status = Status;
        }
    }
}
