using Domain.Interfaces.InterfacesForRepositories;
using Data.Data.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Data.Repositories
{
    public class EventParticipantRepository : IEventParticipantRepository
    {
        private readonly EventContext _context;

        public EventParticipantRepository(EventContext context)
        {
            _context = context;
        }

        public EventParticipant GetById(int id)
        {
            return _context.EventParticipants
                .Include(ep => ep.User)
                .Include(u => u.Event)
                .FirstOrDefault(ep => ep.Id == id);
        }

        public async Task<EventParticipant> GetByIdAsync(int id)
        {
            return await _context.EventParticipants
                .Include(ep => ep.User)
                .Include(u => u.Event)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public void Add(EventParticipant EventParticipant)
        {
            _context.EventParticipants.Add(EventParticipant);
        }

        public IEnumerable<EventParticipant> GetAll()
        {
            return _context.EventParticipants.ToList();
        }

        public async Task<IEnumerable<EventParticipant>> GetAllAsync()
        {
            return await _context.EventParticipants
                .Include(ep => ep.User)
                .Include(u => u.Event)
                .ToListAsync();
        }

        public void Update(EventParticipant EventParticipant)
        {
            _context.EventParticipants.Update(EventParticipant);
        }

        public void Remove(EventParticipant EventParticipant)
        {
            _context.EventParticipants.Remove(EventParticipant);
        }
    }
}
