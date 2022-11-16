
namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Exceptions
{
    public class InvalidExpiryDateException : Exception
    {
        public InvalidExpiryDateException() : base($"ExpiryDate is Invalid") {}
    }
}
