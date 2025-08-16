using Domain.Entities;

namespace Domain.Interfaces.InterfacesForRepositories
{
    public interface IEventRepository
    {
        Event GetById(int id);
        IEnumerable<Event> GetAll();
        void Add(Event Event);
        void Update(Event Event);
        void Remove(Event Event);
    }
}
