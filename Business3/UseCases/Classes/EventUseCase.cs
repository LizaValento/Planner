using AutoMapper;
using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces.InterfacesForUOW;

namespace Application.UseCases.Classes
{
    public class EventUseCase : IEventUseCase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public EventUseCase(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<EventModel> AddAsync(EventModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Event model cannot be null.");
            }
            var EventEntity = _mapper.Map<Event>(model);
            _uow.Events.Add(EventEntity);
            await _uow.CompleteAsync();
            return _mapper.Map<EventModel>(EventEntity);
        }

        public async Task<EventModel?> GetByIdAsync(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Id cannot be null.");
            }

            var Event = await _uow.Events.GetByIdAsync(id.Value);
            return Event == null ? null : _mapper.Map<EventModel>(Event);
        }

        public async Task<List<EventModel>> GetEventsAsync()
        {
            var Events = _uow.Events.GetAll();
            return _mapper.Map<List<EventModel>>(Events);
        }

        public async Task UpdateAsync(EventModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Event model cannot be null.");
            }

            var EventEntity = _mapper.Map<Event>(model);
            _uow.Events.Update(EventEntity);
            await _uow.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
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
