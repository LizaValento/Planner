using Domain.Entities;

namespace Domain.Interfaces.InterfacesForRepositories
{
    public interface IUserRepository
    {
        User GetById(int id);
        Task<User> GetByIdAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
        IEnumerable<User> GetAll();
        void Add(User user);
        void Update(User user);
        void Remove(User user);
    }
}
