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

namespace Application.UseCases.Classes
{
    public class TokenUseCase : ITokenUseCase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly JWTSettings _jwtSettings;

        public TokenUseCase(IUnitOfWork uow, IMapper mapper, IOptions<JWTSettings> jwtSettings)
        {
            _uow = uow;
            _mapper = mapper;
            _jwtSettings = jwtSettings.Value;
        }

        public void CheckAndUpdateTokens()
        {
            var refreshTokenEntities = _uow.RefreshTokens.GetAll();
            foreach (var refreshTokenEntity in refreshTokenEntities)
            {
                if (refreshTokenEntity.ExpiresAt < DateTime.UtcNow)
                {
                    _uow.RefreshTokens.Delete(refreshTokenEntity);
                }
            }

            _uow.Complete();
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
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenLifetime),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        public RefreshTokenModel? GetRefreshToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Token не может быть null или пустым.");
            }

            var refreshTokenEntity = _uow.RefreshTokens.GetByToken(token);
            return refreshTokenEntity == null ? null : _mapper.Map<RefreshTokenModel>(refreshTokenEntity);
        }

        public void Logout(HttpResponse response)
        {
            response.Cookies.Delete("AccessToken");
            response.Cookies.Delete("RefreshToken");
        }

        public TokenModel RefreshToken(string refreshToken, HttpContext httpContext)
        {
            var refreshTokenEntity = GetRefreshToken(refreshToken);

            if (refreshTokenEntity == null || refreshTokenEntity.ExpiresAt < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            var user = _uow.Users.GetById(refreshTokenEntity.UserId);
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

            SaveRefreshToken(user.Id, newRefreshToken);

            var tokenModel = new TokenModel
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

            SetCookies(tokenModel, httpContext);
            return tokenModel;
        }

        public void SaveRefreshToken(int userId, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Token не может быть null или пустым.");
            }

            var refreshTokenEntity = _uow.RefreshTokens.GetByUserId(userId);
            if (refreshTokenEntity != null)
            {
                refreshTokenEntity.Token = token;
                refreshTokenEntity.ExpiresAt = DateTime.UtcNow.AddDays(30);
                _uow.RefreshTokens.Update(refreshTokenEntity);
            }
            else
            {
                var refreshTokenEntityNew = new RefreshToken
                {
                    UserId = userId,
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddDays(30)
                };
                _uow.RefreshTokens.Add(refreshTokenEntityNew);
            }

            _uow.Complete();
        }

        public bool ValidateRefreshToken(int userId, string token)
        {
            var refreshToken = GetRefreshToken(token);
            return refreshToken != null && refreshToken.UserId == userId && refreshToken.ExpiresAt > DateTime.UtcNow;
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

            var accessToken = GenerateAccessToken(claims);
            var refreshToken = GenerateRefreshToken();

            SaveRefreshToken(user.Id, refreshToken);

            var tokenModel = new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            SetCookies(tokenModel, httpContext);
            return tokenModel;
        }
    }
}
