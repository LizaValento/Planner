using AutoMapper;
using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces.InterfacesForUOW;
using Application.UseCases.Interfaces;
using Microsoft.AspNetCore.Http;
using FluentValidation;
using System.Security.Claims;

namespace Application.UseCases.Classes
{
    public class EventUseCase : IEventUseCase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IValidator<EventModel> _eventValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EventUseCase(IUnitOfWork uow, IMapper mapper, IValidator<EventModel> eventValidator,
            IHttpContextAccessor httpContextAccessor)
        {
            _uow = uow;
            _mapper = mapper;
            _eventValidator = eventValidator;
            _httpContextAccessor = httpContextAccessor;
        }

        public (bool Success, IEnumerable<string> Errors) Add(EventModel eventModel)
        {
            if (eventModel == null)
            {
                throw new ArgumentNullException(nameof(eventModel), "Event model cannot be null.");
            }

            var validationResult = _eventValidator.Validate(eventModel);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                return (false, errors);
            }

            eventModel.CreatedBy = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            eventModel.CreatedAt = DateTime.UtcNow;
            eventModel.Users = new List<EventParticipantModel>
            {
                new EventParticipantModel
                {
                    UserId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                    IsOrganizer = true
                }
            };

            var eventEntity = _mapper.Map<Event>(eventModel);
            _uow.Events.Add(eventEntity);

            _uow.Complete();
            return (true, Enumerable.Empty<string>());
        }

        public EventModel? GetById(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Id cannot be null.");
            }

            var Event = _uow.Events.GetById(id.Value);
            return Event == null ? null : _mapper.Map<EventModel>(Event);
        }

        public List<EventModel> GetEvents()
        {
            var Events = _uow.Events.GetAll();
            return _mapper.Map<List<EventModel>>(Events);
        }

        public void Update(EventModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Event model cannot be null.");
            }

            var EventEntity = _mapper.Map<Event>(model);
            _uow.Events.Update(EventEntity);
            _uow.Complete();
        }

        public void Delete(int id)
        {
            var Event = _uow.Events.GetById(id);
            if (Event != null)
            {
                _uow.Events.Remove(Event);
                _uow.Complete();
            }
        }
    }
}
