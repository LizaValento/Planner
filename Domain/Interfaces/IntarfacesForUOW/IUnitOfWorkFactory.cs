using Domain.Interfaces.InterfacesForRepositories;

namespace Domain.Interfaces.InterfacesForUOW
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create();
    }
}
