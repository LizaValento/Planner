using Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Interfaces
{
    public interface IUserUseCase
    {
        UserModel Add(UserModel model);
        UserModel? GetById(int? id);
        List<UserModel> GetUsers();
        void Update(UserModel model);
        void Delete(int id);
        void Register(RegisterModel userModel);
    }
}