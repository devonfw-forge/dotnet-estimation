namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Exceptions
{
    public class TaskNotFoundException : Exception
    {
        public TaskNotFoundException(string id) : base($"Failed to find task matching {id}") { }
    }
}