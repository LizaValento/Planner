using Business.ExceptionHandler;
using Data.Data.Context;
using Data.UOW;
using Domain.Interfaces.InterfacesForRepositories;
using Domain.Interfaces.InterfacesForUOW;
using Microsoft.EntityFrameworkCore;
using System.Text;
using FluentValidation;
using Application.UseCases.Classes;
using Application.UseCases.Interfaces;
using Application.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Application.Validators;
using Data.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
});



// --- UseCases ---
builder.Services.AddScoped<IUserUseCase, UserUseCase>();
builder.Services.AddScoped<IEventUseCase, EventUseCase>();
builder.Services.AddScoped<IEventParticipantUseCase, EventParticipantUseCase>();
builder.Services.AddScoped<ITokenUseCase, TokenUseCase>();

// --- UnitOfWork & Repositories ---
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventParticipantRepository, EventParticipantRepository>();
builder.Services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

// --- FluentValidation ---
builder.Services.AddScoped<IValidator<UserModel>, UserValidator>();
builder.Services.AddScoped<IValidator<EventModel>, EventValidator>();
builder.Services.AddScoped<IValidator<LoginModel>, LoginValidator>();
builder.Services.AddScoped<IValidator<RegisterModel>, RegisterValidator>();

// --- HttpContext и фильтры ---
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped(typeof(ValidateModelAttribute<>));
builder.Services.AddScoped<CustomAuthorizeAttribute>();

// --- Session ---
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<EventContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .LogTo(msg => File.AppendAllText("ef-log.txt", msg + Environment.NewLine),
                  LogLevel.Information)
           .EnableSensitiveDataLogging()
);

builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);


builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JWTSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["AccessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});

// --- Logging ---
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});


builder.Services.AddControllersWithViews();

var app = builder.Build();

// --- Middleware ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();


// --- Routing ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Event}/{action=Main}/{id?}");


app.Run();
