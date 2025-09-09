using Application.DTOs;

namespace Application.UseCases.Interfaces
{
    public interface IEventUseCase
    {
        Task<(bool Success, IEnumerable<string> Errors)> AddAsync(EventModel eventModel);
        Task<EventModel?> GetByIdAsync(int? id);
        Task<List<EventModel>> GetEventsAsync();
        Task UpdateAsync(EventModel model);
        Task DeleteAsync(int id);
    }
}