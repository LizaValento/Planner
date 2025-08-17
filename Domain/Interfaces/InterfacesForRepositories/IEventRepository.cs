using Domain.Entities;

namespace Domain.Interfaces.InterfacesForRepositories
{
    public interface IEventRepository
    {
        Event GetById(int id);
        Task<Event> GetByIdAsync(int id);
        Task<IEnumerable<Event>> GetAllAsync();
        IEnumerable<Event> GetAll();
        void Add(Event Event);
        void Update(Event Event);
        void Remove(Event Event);
    }
}
