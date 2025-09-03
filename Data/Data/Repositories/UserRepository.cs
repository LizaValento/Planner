using Domain.Interfaces.InterfacesForRepositories;
using Data.Data.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EventContext _context;

        public UserRepository(EventContext context)
        {
            _context = context;
        }

        public User GetById(int id)
        {
            return _context.Users
                .Include(x => x.Events)
                .FirstOrDefault(c => c.Id == id);
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(x => x.Events)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(x => x.Events)
                .ToListAsync();
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public void Remove(User user)
        {
            _context.Users.Remove(user);
        }

        public User GetByNickname(string nickname)
        {
            return _context.Users.FirstOrDefault(u => u.Nickname == nickname);
        }
    }
}
