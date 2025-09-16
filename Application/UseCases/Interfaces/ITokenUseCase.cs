using Application.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Interfaces
{
    public interface ITokenUseCase
    {
        void CheckAndUpdateTokens();
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        RefreshTokenModel? GetRefreshToken(string token);
        void Logout(HttpResponse response);
        TokenModel RefreshToken(string refreshToken, HttpContext httpContext);
        void SaveRefreshToken(int userId, string token);
        bool ValidateRefreshToken(int userId, string token);
        void SetCookies(TokenModel tokenModel, HttpContext httpContext);
        TokenModel Authenticate(LoginModel model, HttpContext httpContext);
    }
}
