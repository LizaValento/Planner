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

        public RefreshToken? GetByToken(string token)
        {
            return _context.RefreshTokens.FirstOrDefault(rt => rt.Token == token);
        }

        public IEnumerable<RefreshToken> GetAll()
        {
            return _context.RefreshTokens.ToList();
        }

        public void Add(RefreshToken refreshToken)
        {
            if (refreshToken == null)
                throw new ArgumentNullException(nameof(refreshToken), "RefreshToken не может быть null.");

            try
            {
                _context.RefreshTokens.Add(refreshToken);
                _context.SaveChanges(); // синхронный вызов
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Ошибка при добавлении RefreshToken в базу данных.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Неизвестная ошибка при добавлении RefreshToken.", ex);
            }
        }

        public void Update(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            _context.SaveChanges();
        }

        public void Delete(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Remove(refreshToken);
            _context.SaveChanges();
        }

        public RefreshToken? GetByUserId(int userId)
        {
            return _context.RefreshTokens.FirstOrDefault(rt => rt.UserId == userId);
        }
    }
}
