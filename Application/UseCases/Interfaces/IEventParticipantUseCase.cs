using Application.DTOs;

namespace Application.UseCases.Interfaces
{
    public interface IEventParticipantUseCase
    {
        Task<EventParticipantModel> AddAsync(EventParticipantModel model);
        Task<EventParticipantModel?> GetByIdAsync(int? id);
        Task<List<EventParticipantModel>> GetEventParticipantsAsync();
        Task UpdateAsync(EventParticipantModel model);
        Task DeleteAsync(int id);
    }
}