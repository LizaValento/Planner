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
    }
}
