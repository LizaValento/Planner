using Domain.Entities;

namespace Application.DTOs
{
    public class EventModel
    {
        public EventModel() { }
        public EventModel(Event Event)
        {
            Id = Event.Id;
            Title = Event.Title;
            Description = Event.Description;
            Date = Event.Date;
            Location = Event.Location;
            CreatedBy = Event.CreatedBy;
            CreatedAt = Event.CreatedAt;
            EventParticipants = Event.EventParticipants?.Select(b => new EventParticipantModel(b)).ToList() ?? new List<EventParticipantModel>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<EventParticipantModel> EventParticipants { get; set; }
    }
}
