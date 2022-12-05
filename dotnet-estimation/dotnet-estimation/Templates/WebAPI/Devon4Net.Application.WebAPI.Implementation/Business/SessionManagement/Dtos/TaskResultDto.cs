using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos
{
    public class TaskResultDto
    {
        public string Id { get; set; }

        public int AmountOfVotes { get; set; }

        public float ComplexityAverage { get; set; }

        public int? FinalValue { get; set; }
    }
}