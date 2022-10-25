namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos
{
    public class StatusDto
    {
        public bool IsValid { get; set; }

        public TaskDto? CurrentTask { get; set; }
    }
}