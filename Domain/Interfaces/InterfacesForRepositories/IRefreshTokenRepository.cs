using Domain.Entities;

namespace Domain.Interfaces.InterfacesForRepositories
{
    public interface IRefreshTokenRepository
    {
        RefreshToken? GetByToken(string token);
        IEnumerable<RefreshToken> GetAll();
        void Add(RefreshToken refreshToken);
        void Update(RefreshToken refreshToken);
        void Delete(RefreshToken refreshToken);
        RefreshToken? GetByUserId(int userId);
    }
}
