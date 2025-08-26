using Application.DTOs;

namespace Application.UseCases.Interfaces
{
    public interface IEventUseCase
    {
        Task<EventModel> AddAsync(EventModel model);
        Task<EventModel?> GetByIdAsync(int? id);
        Task<List<EventModel>> GetEventsAsync();
        Task UpdateAsync(EventModel model);
        Task DeleteAsync(int id);
    }
}