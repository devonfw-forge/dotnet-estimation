using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Converters
{
    public class UserConverter
    {
        public static UserDto ModelToDto(Devon4Net.Application.WebAPI.Implementation.Domain.Entities.User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
            };
        }
    }
}
