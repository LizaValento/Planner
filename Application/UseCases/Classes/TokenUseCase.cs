using AutoMapper;
using Application.DTOs;
using Application.UseCases.Interfaces;
using Domain.Entities;
using Domain.Interfaces.InterfacesForUOW;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Application.UseCases.Interfaces;

namespace Application.UseCases.Classes
{
    public class TokenUseCase : ITokenUseCase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly JWTSettings _jwtSettings;
        private readonly IUserUseCase _userUseCase;
        public TokenUseCase(IUnitOfWork uow, IMapper mapper, IOptions<JWTSettings> jwtSettings, IUserUseCase userUseCase)
        {
            _uow = uow;
            _mapper = mapper;
            _jwtSettings = jwtSettings.Value;
            _userUseCase = userUseCase;
        }

        public async Task CheckAndUpdateTokens()
        {
            var refreshTokenEntities = await _uow.RefreshTokens.GetAllAsync();
            foreach (var refreshTokenEntity in refreshTokenEntities)
            {
                if (refreshTokenEntity.ExpiresAt < DateTime.UtcNow)
                {
                    _uow.RefreshTokens.DeleteAsync(refreshTokenEntity);
                }
            }

            await _uow.CompleteAsync();
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            if (claims == null || !claims.Any())
            {
                throw new ArgumentNullException(nameof(claims), "Claims не могут быть null или пустыми.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.AccessTokenLifetime),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task<RefreshTokenModel> GetRefreshToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Token не может быть null или пустым.");
            }

            var refreshTokenEntity = await _uow.RefreshTokens.GetByTokenAsync(token);
            if (refreshTokenEntity == null)
            {
                return null;
            }
            return _mapper.Map<RefreshTokenModel>(refreshTokenEntity);
        }

        public void Logout(HttpResponse response)
        {
            response.Cookies.Delete("AccessToken");
            response.Cookies.Delete("RefreshToken");
        }

        public async Task<TokenModel> RefreshToken(string refreshToken, HttpContext httpContext)
        {
            var refreshTokenEntity = await GetRefreshToken(refreshToken);

            if (refreshTokenEntity == null || refreshTokenEntity.ExpiresAt < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            var user = await _uow.Users.GetByIdAsync(refreshTokenEntity.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Nickname),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var newAccessToken = GenerateAccessToken(claims);
            var newRefreshToken = GenerateRefreshToken();

            await SaveRefreshToken(user.Id, newRefreshToken);

            _userUseCase.SetCookies(new TokenModel
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            }, httpContext);

            return new TokenModel
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task SaveRefreshToken(int userId, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Token не может быть null или пустым.");
            }

            var refreshTokenEntity = await _uow.RefreshTokens.GetByUserIdAsync(userId);
            if (refreshTokenEntity != null)
            {
                refreshTokenEntity.Token = token;
                refreshTokenEntity.ExpiresAt = DateTime.UtcNow.AddDays(30);
                _uow.RefreshTokens.UpdateAsync(refreshTokenEntity);
            }
            else
            {
                var refreshToken = new RefreshTokenModel
                {
                    UserId = userId,
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddDays(30)
                };

                var refreshTokenEntityNew = new RefreshToken
                {
                    UserId = refreshToken.UserId,
                    Token = refreshToken.Token,
                    ExpiresAt = refreshToken.ExpiresAt
                };
                _uow.RefreshTokens.AddAsync(refreshTokenEntityNew);
            }

            await _uow.CompleteAsync();
        }

        public async Task<bool> ValidateRefreshToken(int userId, string token)
        {
            var refreshToken = await GetRefreshToken(token);
            return refreshToken != null && refreshToken.UserId == userId && refreshToken.ExpiresAt > DateTime.UtcNow;
        }
    }
}
