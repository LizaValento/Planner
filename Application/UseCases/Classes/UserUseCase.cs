using AutoMapper;
using Application.DTOs;
using Application.UseCases.Interfaces;
using Domain.Entities;
using Domain.Interfaces.InterfacesForUOW;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.UseCases.Classes
{
    public class UserUseCase : IUserUseCase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ITokenUseCase _tokenUseCase;

        public UserUseCase(IUnitOfWork uow, IMapper mapper, ITokenUseCase tokenUseCase)
        {
            _uow = uow;
            _mapper = mapper;
            _tokenUseCase = tokenUseCase;
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

        public void SetCookies(TokenModel tokenModel, HttpContext httpContext)
        {
            httpContext.Response.Cookies.Append("AccessToken", tokenModel.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(1)
            });

            httpContext.Response.Cookies.Append("RefreshToken", tokenModel.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });
        }

        public void Register(RegisterModel userModel)
        {
            if (userModel == null)
            {
                throw new ArgumentNullException(nameof(userModel), "Register model cannot be null.");
            }

            var existingUser = _uow.Users.GetByNickname(userModel.Nickname);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User already exists.");
            }

            var userEntity = new User
            {
                FirstName = userModel.FirstName,
                Password = userModel.Password,
                LastName = userModel.LastName,
                Nickname = userModel.Nickname,
                Role = "User"
            };

            _uow.Users.Add(userEntity);
            _uow.Complete();
        }

        public TokenModel Authenticate(LoginModel model, HttpContext httpContext)
        {
            var user = _uow.Users.GetByNickname(model.Nickname);
            if (user == null || user.Password != model.Password)
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Nickname),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var accessToken = _tokenUseCase.GenerateAccessToken(claims);
            var refreshToken = _tokenUseCase.GenerateRefreshToken();

            _tokenUseCase.SaveRefreshToken(user.Id, refreshToken);

            SetCookies(new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            }, httpContext);

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
