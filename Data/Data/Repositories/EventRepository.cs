using Domain.Interfaces.InterfacesForRepositories;
using Data.Data.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Data.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly EventContext _context;

        public EventRepository(EventContext context)
        {
            _context = context;
        }

        public Event GetById(int id)
        {
            return _context.Events
                .Include(x => x.Users)
                .FirstOrDefault(c => c.Id == id);
        }

        public async Task<Event> GetByIdAsync(int id)
        {
            return await _context.Events
                .Include(x => x.Users)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public void Add(Event Event)
        {
            _context.Events.Add(Event);
        }

        public IEnumerable<Event> GetAll()
        {
            return _context.Events.ToList();
        }

        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            return await _context.Events
                .Include(x => x.Users)
                .ToListAsync();
        }

        public void Update(Event Event)
        {
            _context.Events.Update(Event);
        }

        public void Remove(Event Event)
        {
            _context.Events.Remove(Event);
        }
    }
}
