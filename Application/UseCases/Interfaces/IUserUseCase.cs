using Application.DTOs;

namespace Application.UseCases.Interfaces
{
    public interface IUserUseCase
    {
        Task<UserModel> AddAsync(UserModel model);
        Task<UserModel?> GetByIdAsync(int? id);
        Task<List<UserModel>> GetUsersAsync();
        Task UpdateAsync(UserModel model);
        Task DeleteAsync(int id);
    }
}