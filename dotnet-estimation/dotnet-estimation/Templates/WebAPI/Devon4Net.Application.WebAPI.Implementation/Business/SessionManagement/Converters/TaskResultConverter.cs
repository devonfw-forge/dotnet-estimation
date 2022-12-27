using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Converters
{
    public class TaskResultConverter
    {
        public static TaskResultDto ModelToDto(TaskStatusChangeDto task)
        {
            return new TaskResultDto
            {
                Id = task.Id,
                AmountOfVotes = task.Result.AmountOfVotes,
                ComplexityAverage = task.Result.ComplexityAverage,
                FinalValue = task.Result.FinalValue
            };
        }
    }
}