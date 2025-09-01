using Domain.Entities;

namespace Domain.Interfaces.InterfacesForRepositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByTokenAsync(string token);
        Task AddAsync(RefreshToken refreshToken);
        Task UpdateAsync(RefreshToken refreshToken);
        Task DeleteAsync(RefreshToken refreshToken);
        Task<IEnumerable<RefreshToken>> GetAllAsync();
        Task<RefreshToken> GetByUserIdAsync(int userId);
    }
}
