using Business.Models;
using Data.Entities;

namespace Business.Service.Interfaces
{
    public interface IEventParticipantService
    {
        EventParticipant GetById(int id);
        List<EventParticipant> GetEventParticipants();
        void AddEventParticipant(EventParticipantModel EventParticipant);
        void UpdateEventParticipant(EventParticipantModel EventParticipant);
        void DeleteEventParticipant(int id);
    }
}
