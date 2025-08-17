using AutoMapper;
using Application.DTOs;
using Domain.Entities;

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
