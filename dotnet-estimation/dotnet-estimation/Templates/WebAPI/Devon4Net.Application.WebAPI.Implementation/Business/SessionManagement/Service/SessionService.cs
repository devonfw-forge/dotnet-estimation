using System.Linq.Expressions;
using Devon4Net.Infrastructure.LiteDb.Repository;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    public class SessionService: ISessionService
    {
        private readonly ILiteDbRepository<Session> _sessionRepository;

        public SessionService(ILiteDbRepository<Session> SessionRepository) 
        {
            _sessionRepository = SessionRepository;
        }

        public async Task<Session> GetSession(long id)
        {
            var expression = LiteDB.Query.EQ("_id", id);
            
            return _sessionRepository.GetFirstOrDefault(expression);
        }
    }
}