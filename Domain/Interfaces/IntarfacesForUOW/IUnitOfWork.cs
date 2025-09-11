using Domain.Interfaces.InterfacesForRepositories;

namespace Domain.Interfaces.InterfacesForUOW
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IEventRepository Events { get; } 
        IEventParticipantRepository EventParticipants { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        int Complete();
        Task<int> CompleteAsync();
    }
}
