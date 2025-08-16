using Business.Models;
using Data.Entities;

namespace Business.Service.Interfaces
{
    public interface IEventService
    {
        Event GetById(int id);
        List<Event> GetEvents();
        void AddEvent(EventModel Event);
        void UpdateEvent(EventModel Event);
        void DeleteEvent(int id);
    }
}
