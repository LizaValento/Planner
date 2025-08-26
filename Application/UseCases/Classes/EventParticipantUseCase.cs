using AutoMapper;
using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces.InterfacesForUOW;
using Application.UseCases.Interfaces;

namespace Application.UseCases.Classes
{
    public class EventParticipantUseCase : IEventParticipantUseCase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public EventParticipantUseCase(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<EventParticipantModel> AddAsync(EventParticipantModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "EventParticipant model cannot be null.");
            }
            var EventParticipantEntity = _mapper.Map<EventParticipant>(model);
            _uow.EventParticipants.Add(EventParticipantEntity);
            await _uow.CompleteAsync();
            return _mapper.Map<EventParticipantModel>(EventParticipantEntity);
        }

        public async Task<EventParticipantModel?> GetByIdAsync(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Id cannot be null.");
            }

            var EventParticipant = await _uow.EventParticipants.GetByIdAsync(id.Value);
            return EventParticipant == null ? null : _mapper.Map<EventParticipantModel>(EventParticipant);
        }

        public async Task<List<EventParticipantModel>> GetEventParticipantsAsync()
        {
            var EventParticipants = _uow.EventParticipants.GetAll();
            return _mapper.Map<List<EventParticipantModel>>(EventParticipants);
        }

        public async Task UpdateAsync(EventParticipantModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "EventParticipant model cannot be null.");
            }

            var EventParticipantEntity = _mapper.Map<EventParticipant>(model);
            _uow.EventParticipants.Update(EventParticipantEntity);
            await _uow.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var EventParticipant = _uow.EventParticipants.GetById(id);
            if (EventParticipant != null)
            {
                _uow.EventParticipants.Remove(EventParticipant);
                _uow.Complete();
            }
        }
    }
}
