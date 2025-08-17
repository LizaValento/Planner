using Domain.Entities;

namespace Domain.Interfaces.InterfacesForRepositories
{
    public interface IEventParticipantRepository
    {
        EventParticipant GetById(int id);
        IEnumerable<EventParticipant> GetAll();
        Task<EventParticipant> GetByIdAsync(int id);
        Task<IEnumerable<EventParticipant>> GetAllAsync();
        void Add(EventParticipant EventParticipant);
        void Update(EventParticipant EventParticipant);
        void Remove(EventParticipant EventParticipant);
    }
}
