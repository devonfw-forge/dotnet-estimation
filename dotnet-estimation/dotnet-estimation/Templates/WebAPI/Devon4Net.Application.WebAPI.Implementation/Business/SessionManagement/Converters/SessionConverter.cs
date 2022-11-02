using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dto;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Converters
{
    /// <summary>
    /// EmployeeConverter
    /// </summary>
    public static class SessionConverter
    {
        /// <summary>
        /// ModelToDto transformation
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static SessionDto ModelToDto(Session item)
        {
            if (item == null) return new SessionDto();

            return new SessionDto
            {
                Id = item.Id,
                InviteToken = item.InviteToken,
                ExpiresAt = item.ExpiresAt,
                Tasks = item.Tasks,
                Users = item.Users,
            };
        }

        /// <summary>
        /// DtoToModel transformation
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Session DtoToModel(SessionDto item)
        {
            if (item == null) return new Session();

            return new Session
            {
                Id = item.Id,
                InviteToken = item.InviteToken,
                ExpiresAt = item.ExpiresAt,
                Tasks = item.Tasks,
                Users = item.Users,
            };
        }
    }
}
