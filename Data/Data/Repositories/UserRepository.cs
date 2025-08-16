using Domain.Interfaces.InterfacesForRepositories;
using Data.Data.LibraryContext;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Context _context;

        public UserRepository(Context context)
        {
            _context = context;
        }

        public User GetById(int id)
        {
            return _context.Users
                .Include(x => x.Books)
                .ThenInclude(b => b.Author)
                .FirstOrDefault(c => c.Id == id);
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(x => x.Books)
                    .ThenInclude(b => b.Author)
                .FirstOrDefaultAsync(c => c.Id == id);
        }


        public User GetByNickname(string nickname)
        {
            return _context.Users.FirstOrDefault(u => u.Nickname == nickname);
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public void Remove(User user)
        {
            _context.Users.Remove(user);
        }

        public (List<Book> Books, int TotalCount) GetUserBooks(int userId, int page, int pageSize)
        {
            var books = _context.Books
                .Where(b => b.UserId == userId)
                .Include(b => b.Author)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalCount = _context.Books.Count(b => b.UserId == userId);

            return (books, totalCount);
        }
    }
}
