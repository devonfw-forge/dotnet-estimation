
using Devon4Net.Infrastructure.Logger.Logging;
using Devon4Net.Infrastructure.LiteDb.Repository;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dto;
using LiteDB;
using System.Security.Cryptography;
namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    /// <summary>
    /// Session service implementation
    /// </summary>
    public class SessionService: ISessionService
    {
        //private readonly ISessionRepository _sessionRepository;
        private readonly ILiteDbRepository<Session> _sessionRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="SessionRepository"></param>
        public SessionService(ILiteDbRepository<Session> SessionRepository) 
        {
            _sessionRepository = SessionRepository;
        }

        /// <summary>
        /// Creates the Session
        /// </summary>
        /// <param name="sessionDto"></param>
        /// <returns></returns>
        public async Task<BsonValue> CreateSession(SessionDto sessionDto)
        {
            Devon4NetLogger.Debug($"CreateSession method from service SessionService with value : {sessionDto.ExpiresAt}");

            return _sessionRepository.Create(new Session{
                InviteToken = generateToken(),
                ExpiresAt = sessionDto.ExpiresAt
            });
        }
        private string generateToken()
        {   //generates 8 random bytes and returns them as a token string 
            byte[] randomNumber = new byte[8];
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            rngCsp.GetNonZeroBytes(randomNumber);
            return BitConverter.ToString(randomNumber);
        }
    }
}