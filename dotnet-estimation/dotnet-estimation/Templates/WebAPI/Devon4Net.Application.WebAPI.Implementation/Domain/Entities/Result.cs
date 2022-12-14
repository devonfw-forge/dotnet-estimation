using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Devon4Net.Application.WebAPI.Implementation.Domain.Entities
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public partial class Result
    {
        public int AmountOfVotes { get; set; }

        public float ComplexityAverage { get; set; }

        public int? FinalValue { get; set; }
    }
}