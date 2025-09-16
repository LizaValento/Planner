using Application.DTOs;

namespace Application.UseCases.Interfaces
{
    public interface IEventUseCase
    {
        (bool Success, IEnumerable<string> Errors) Add(EventModel eventModel);
        EventModel? GetById(int? id);
        List<EventModel> GetEvents();
        void Update(EventModel model);
        void Delete(int id);
    }
}
