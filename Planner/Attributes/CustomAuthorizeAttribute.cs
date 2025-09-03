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
            var getRefreshTokenUseCase = scope.ServiceProvider.GetRequiredService<GetRefreshTokenUseCase>();
            var getUserByIdUseCase = scope.ServiceProvider.GetRequiredService<GetUserByIdUseCase>();

            if (string.IsNullOrEmpty(refreshToken) || !await ValidateRefreshTokenAsync(getRefreshTokenUseCase, refreshToken))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (string.IsNullOrEmpty(token) || !ValidateAccessToken(token))
            {
                var userId = await GetUserIdFromRefreshTokenAsync(getRefreshTokenUseCase, refreshToken);
                if (userId == null)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                var user = await getUserByIdUseCase.ExecuteAsync(userId.Value);
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Nickname),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var newAccessToken = _generateAccessTokenUseCase.Execute(claims);

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

    private async Task<bool> ValidateRefreshTokenAsync(GetRefreshTokenUseCase getRefreshTokenUseCase, string refreshToken)
    {
        var refreshTokenEntity = await getRefreshTokenUseCase.ExecuteAsync(refreshToken);
        return refreshTokenEntity != null && refreshTokenEntity.ExpiresAt > DateTime.UtcNow;
    }

    private async Task<int?> GetUserIdFromRefreshTokenAsync(GetRefreshTokenUseCase getRefreshTokenUseCase, string refreshToken)
    {
        var refreshTokenEntity = await getRefreshTokenUseCase.ExecuteAsync(refreshToken);
        return refreshTokenEntity?.UserId;
    }
}
