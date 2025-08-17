using Domain.Interfaces.InterfacesForRepositories;
using Domain.Interfaces.InterfacesForUOW;
using Data.Data.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Data.UOW
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public UnitOfWorkFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IUnitOfWork Create()
        {
            var context = _serviceProvider.GetRequiredService<EventContext>();
            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            var eventRepository = _serviceProvider.GetRequiredService<IEventRepository>();
            var eventParticipantRepository = _serviceProvider.GetRequiredService<IEventParticipantRepository>();

            return new UnitOfWork(context, userRepository, eventRepository, eventParticipantRepository);
        }
    }

}
