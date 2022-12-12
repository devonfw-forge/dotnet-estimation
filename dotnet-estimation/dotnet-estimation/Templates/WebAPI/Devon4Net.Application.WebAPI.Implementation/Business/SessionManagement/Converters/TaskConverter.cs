using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Converters
{
    public class TaskConverter
    {
        public static TaskDto ModelToDto(Devon4Net.Application.WebAPI.Implementation.Domain.Entities.Task task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Url = task.Url,
                Status = task.Status,
                Estimations = task.Estimations.Select(item => EstimationConverter.ModelToDto(item)).ToList()
            };
        }
    }
}
