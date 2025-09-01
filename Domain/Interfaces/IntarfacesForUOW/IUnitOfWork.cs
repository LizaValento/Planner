using Domain.Interfaces.InterfacesForRepositories;

namespace Domain.Interfaces.InterfacesForUOW
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IEventRepository Events { get; } 
        IEventParticipantRepository EventParticipants { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        int Complete();
        Task<int> CompleteAsync();
    }
}
