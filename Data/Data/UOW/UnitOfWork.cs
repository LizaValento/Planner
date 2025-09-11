using Domain.Interfaces.InterfacesForRepositories;
using Domain.Interfaces.InterfacesForUOW;
using Data.Data.Context;

namespace Data.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EventContext _context;

        public IUserRepository Users { get; }
        public IEventRepository Events { get; }
        public IEventParticipantRepository EventParticipants { get; }
        public IRefreshTokenRepository RefreshTokens { get; }

        public UnitOfWork(
            EventContext context,
            IUserRepository users,
            IEventRepository events,
            IEventParticipantRepository eventParticipants,
            IRefreshTokenRepository refreshTokens)
        {
            _context = context;
            Users = users;
            Events = events;
            EventParticipants = eventParticipants;
            RefreshTokens = refreshTokens;
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // ❌ Не трогаем Dispose — DbContext освобождается DI контейнером
    }
}
