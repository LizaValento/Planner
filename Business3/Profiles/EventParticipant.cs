using AutoMapper;
using Business.Models;
using Data.Entities;

namespace Business.Profiles
{
    public class EventParticipantProfile : Profile
    {
        public EventParticipantProfile()
        {
            CreateMap<EventParticipantModel, EventParticipant>();
            CreateMap<EventParticipant, EventParticipantModel>();
        }
    }
}
