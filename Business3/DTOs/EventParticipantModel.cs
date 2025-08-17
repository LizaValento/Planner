using Domain.Entities;

namespace Application.DTOs
{
    public class EventParticipantModel
    {
        public EventParticipantModel() { }
        public EventParticipantModel(EventParticipant eventParticipantModel)
        {
            Id = eventParticipantModel.Id;
            EventId = eventParticipantModel.EventId;
            UserId = eventParticipantModel.UserId;
            IsOrganizer = eventParticipantModel.IsOrganizer;
        }
        public int Id { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public bool IsOrganizer { get; set; }
    }
}
