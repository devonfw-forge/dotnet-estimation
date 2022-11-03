namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Exceptions
{
    public class InvalidSessionException : Exception
    {
        public InvalidSessionException(long id) : base($"Session with ID {id} is invalid.") { }
    }
}