using Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Interfaces
{
    public interface IUserUseCase
    {
        Task<UserModel> AddAsync(UserModel model);
        Task<UserModel?> GetByIdAsync(int? id);
        Task<List<UserModel>> GetUsersAsync();
        Task UpdateAsync(UserModel model);
        Task DeleteAsync(int id);
        void SetCookies(TokenModel tokenModel, HttpContext httpContext);
        void Register(RegisterModel userModel);
        TokenModel Authenticate(LoginModel model, HttpContext httpContext);
    }
}