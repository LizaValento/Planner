using AutoMapper;
using Application.DTOs;         
using Domain.Entities;
using Domain.Interfaces.InterfacesForUOW;

namespace Application.UseCases.Classes
{
    public class UserUseCase : IUserUseCase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UserUseCase(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<UserModel> AddAsync(UserModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "User model cannot be null.");
            }
            var userEntity = _mapper.Map<User>(model);
            _uow.Users.Add(userEntity);
            await _uow.CompleteAsync();
            return _mapper.Map<UserModel>(userEntity);
        }

        public async Task<UserModel?> GetByIdAsync(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Id cannot be null.");
            }

            var user = await _uow.Users.GetByIdAsync(id.Value);
            return user == null ? null : _mapper.Map<UserModel>(user);
        }

        public async Task<List<UserModel>> GetUsersAsync()
        {
            var users = _uow.Users.GetAll();
            return _mapper.Map<List<UserModel>>(users);
        }

        public async Task UpdateAsync(UserModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "User model cannot be null.");
            }

            var userEntity = _mapper.Map<User>(model);
            _uow.Users.Update(userEntity);
            await _uow.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = _uow.Users.GetById(id);
            if (user != null)
            {
                _uow.Users.Remove(user);
                _uow.Complete();
            }
        }
    }
}
