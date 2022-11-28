using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Converters
{
    public class EstimationConverter
    {
        public static EstimationDto ModelToDto(Devon4Net.Application.WebAPI.Implementation.Domain.Entities.Estimation estimation)
        {
            return new EstimationDto
            {
                TaskId = "",
                Complexity = estimation.Complexity,
                VoteBy = estimation.VoteBy,
            };
        }
    }
}
