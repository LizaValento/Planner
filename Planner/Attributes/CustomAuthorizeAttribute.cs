using Application.DTOs;
using Application.UseCases.Interfaces;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class CustomAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly JWTSettings _jwtSettings;
    private readonly IMapper _mapper;
    private readonly ITokenUseCase _tokenUseCase;

    public CustomAuthorizeAttribute(IOptions<JWTSettings> jwtSettings, IMapper mapper, ITokenUseCase tokenUseCase)
    {
        _jwtSettings = jwtSettings.Value;
        _mapper = mapper;
        _tokenUseCase = tokenUseCase;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var token = context.HttpContext.Request.Cookies["AccessToken"];
        var refreshToken = context.HttpContext.Request.Cookies["RefreshToken"];

        using (var scope = context.HttpContext.RequestServices.CreateScope())
        {
            //var getRefreshTokenUseCase = scope.ServiceProvider.GetRequiredService<ITokenUseCase>();
            var tokenUseCase = scope.ServiceProvider.GetRequiredService<ITokenUseCase>();
            var userUseCase = scope.ServiceProvider.GetRequiredService<IUserUseCase>();
            //var getUserByIdUseCase = scope.ServiceProvider.GetRequiredService<GetUserByIdUseCase>();

            if (string.IsNullOrEmpty(refreshToken) || !await ValidateRefreshTokenAsync(tokenUseCase, refreshToken))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (string.IsNullOrEmpty(token) || !ValidateAccessToken(token))
            {
                var userId = await GetUserIdFromRefreshTokenAsync(tokenUseCase, refreshToken);
                if (userId == null)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                var user = await userUseCase.GetByIdAsync(userId.Value);
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Nickname),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var newAccessToken = tokenUseCase.GenerateAccessToken(claims);

                context.HttpContext.Response.Cookies.Append("AccessToken", newAccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
            }
        }
    }

    private bool ValidateAccessToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ClockSkew = TimeSpan.Zero
            };

            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> ValidateRefreshTokenAsync(ITokenUseCase tokenUseCase, string refreshToken)
    {
        var refreshTokenEntity = await tokenUseCase.GetRefreshToken(refreshToken);
        return refreshTokenEntity != null && refreshTokenEntity.ExpiresAt > DateTime.UtcNow;
    }

    private async Task<int?> GetUserIdFromRefreshTokenAsync(ITokenUseCase tokenUseCase, string refreshToken)
    {
        var refreshTokenEntity = await tokenUseCase.GetRefreshToken(refreshToken);
        return refreshTokenEntity?.UserId;
    }
}
