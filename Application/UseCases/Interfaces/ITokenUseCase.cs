using Application.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Interfaces
{
    public interface ITokenUseCase
    {
        Task CheckAndUpdateTokens();
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        RefreshTokenModel GetRefreshToken(string token);
        void Logout(HttpResponse response);
        Task<TokenModel> RefreshToken(string refreshToken, HttpContext httpContext);
        Task SaveRefreshToken(int userId, string token);
        Task<bool> ValidateRefreshToken(int userId, string token);
    }
}