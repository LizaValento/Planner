using AutoMapper;
using Application.DTOs;
using Domain.Entities;

namespace Business.Profiles
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<EventModel, Event>();
            CreateMap<Event, EventModel>();
        }
    }
}
