using Domain.Interfaces.InterfacesForRepositories;
using Data.Data.Repositories;
using Domain.Interfaces.InterfacesForUOW;
using Data.Data.Context;

namespace Data.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EventContext _context;
        public IUserRepository Users { get; private set; }
        public IEventRepository Events { get; private set; }
        public IEventParticipantRepository EventParticipants { get; private set; }

        public UnitOfWork(EventContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Events = new EventRepository(_context);
            EventParticipants = new EventParticipantRepository(_context);
        }

        public UnitOfWork(EventContext context, IUserRepository userRepository, IEventRepository eventRepository, IEventParticipantRepository eventParticipantRepository)
        {
            _context = context;
            Users = userRepository;
            Events = eventRepository;
            EventParticipants = eventParticipantRepository;
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
