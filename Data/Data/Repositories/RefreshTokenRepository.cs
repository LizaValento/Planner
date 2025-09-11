using Domain.Interfaces.InterfacesForRepositories;
using Data.Data.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Data.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly EventContext _context;

        public RefreshTokenRepository(EventContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<IEnumerable<RefreshToken>> GetAllAsync()
        {
            return await _context.RefreshTokens.ToListAsync();
        }

        public void Add(RefreshToken refreshToken)
        {
            try
            {
                //Console.WriteLine("DB CanConnect = " + _context.Database.CanConnect());
                _context.RefreshTokens.Add(refreshToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] " + ex.GetType().Name + ": " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public void Update(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
        }

        public async Task DeleteAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Remove(refreshToken);
        }

        public async Task<RefreshToken> GetByUserIdAsync(int userId)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);
        }
    }
}
