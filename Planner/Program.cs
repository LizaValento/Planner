using Business.ExceptionHandler;
using Data.Data.Context;
using Data.Data.Repositories;
using Data.UOW;
using Domain.Interfaces.InterfacesForRepositories;
using Domain.Interfaces.InterfacesForUOW;
using Microsoft.EntityFrameworkCore;
using Application.Validators;
using FluentValidation.AspNetCore;
using Application.Services.Classes;
using Application.DTOs;
using Application.Profiles;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Application.UseCases.AuthorCase;
using Application.UseCases.BookCase;
using Application.UseCases.UserCase;
using Application.UseCases.TokenCase;
using Application.UseCases.BookReturnCase;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddAutoMapper(typeof(UserProfile));
builder.Configuration.AddJsonFile("dbsettings.json", optional: false, reloadOnChange: true);
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<Context>(options =>
    options.UseInMemoryDatabase("Library"));

builder.Services.AddControllersWithViews()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<AuthorValidator>();
        fv.RegisterValidatorsFromAssemblyContaining<BookValidator>();
        fv.RegisterValidatorsFromAssemblyContaining<UserValidator>();
        fv.RegisterValidatorsFromAssemblyContaining<LoginValidator>();
        fv.RegisterValidatorsFromAssemblyContaining<RegisterValidator>();
    });
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<GetUserByIdUseCase>();
builder.Services.AddScoped<GetUsersUseCase>();
builder.Services.AddScoped<AddUserUseCase>();
builder.Services.AddScoped<UpdateUserUseCase>();
builder.Services.AddScoped<DeleteUserUseCase>();
builder.Services.AddScoped<RegisterUserUseCase>();
builder.Services.AddScoped<AuthenticateUserUseCase>();
builder.Services.AddScoped<GetUserBooksUseCase>();

builder.Services.AddScoped<AddBookUseCase>();
builder.Services.AddScoped<GetBookByIdUseCase>();
builder.Services.AddScoped<GetFreeBooksUseCase>();
builder.Services.AddScoped<UpdateBookUseCase>();
builder.Services.AddScoped<DeleteBookUseCase>();
builder.Services.AddScoped<TakeBookUseCase>();
builder.Services.AddScoped<SearchBooksByTitleUseCase>();
builder.Services.AddScoped<SearchBookByISBNUseCase>();
builder.Services.AddScoped<GetAllBooksByGenresUseCase>();
builder.Services.AddScoped<GetAllBooksByAuthorUseCase>();
builder.Services.AddScoped<GetBooksForPaginationUseCase>();
builder.Services.AddScoped<GetPagedBooksUseCase>();
builder.Services.AddScoped<SetCookiesUseCase>();

builder.Services.AddScoped<AddAuthorUseCase>();
builder.Services.AddScoped<UpdateAuthorUseCase>();
builder.Services.AddScoped<DeleteAuthorUseCase>();
builder.Services.AddScoped<GetAuthorByIdUseCase>();
builder.Services.AddScoped<GetAuthorsPaginationUseCase>();
builder.Services.AddScoped<GetAuthorByIdForBooksUseCase>();

builder.Services.AddScoped<GenerateAccessTokenUseCase>();
builder.Services.AddScoped<GenerateRefreshTokenUseCase>();
builder.Services.AddScoped<SaveRefreshTokenUseCase>();
builder.Services.AddScoped<GetRefreshTokenUseCase>();
builder.Services.AddScoped<ValidateRefreshTokenUseCase>();
builder.Services.AddScoped<RefreshTokenUseCase>();
builder.Services.AddScoped<CheckAndUpdateTokensUseCase>();
builder.Services.AddScoped<LogoutUseCase>();

builder.Services.AddScoped<IValidator<BookModel>, BookValidator>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<CheckReturnDatesUseCase>();
builder.Services.AddScoped<BookReturnBackgroundServiceUseCase>();
builder.Services.AddHostedService<BookReturnHostedService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CustomAuthorizeAttribute>();
builder.Services.AddScoped(typeof(ValidateModelAttribute<>));

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});
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


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("role", "Admin"));
});

builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

var app = builder.Build();


app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty;
});
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
app.UseMiddleware<TokenMiddleware>();
app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Book}/{action=Main}/{id?}");

app.MapRazorPages();

app.Run();