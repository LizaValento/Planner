using Domain.Entities;

namespace Domain.Interfaces.InterfacesForRepositories
{
    public interface IUserRepository
    {
        User GetById(int id);
        IEnumerable<User> GetAll();
        void Add(User user);
        void Update(User user);
        void Remove(User user);
    }
}
