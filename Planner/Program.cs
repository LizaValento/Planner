using Business.ExceptionHandler;
using Data.Data.Context;
using Data.UOW;
using Domain.Interfaces.InterfacesForRepositories;
using Domain.Interfaces.InterfacesForUOW;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FluentValidation;
using Application.UseCases.Classes;
using Application.UseCases.Interfaces;
using Application.DTOs;
using Application.Profiles;
using AutoMapper;
using Application.Validators;
using Data.Data.Repositories;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
});


// --- DbContext ---
builder.Services.AddDbContext<EventContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- UseCases ---
builder.Services.AddScoped<IUserUseCase, UserUseCase>();
builder.Services.AddScoped<IEventUseCase, EventUseCase>();
builder.Services.AddScoped<IEventParticipantUseCase, EventParticipantUseCase>();

// --- UnitOfWork & Repositories ---
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventParticipantRepository, EventParticipantRepository>();
builder.Services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

// --- FluentValidation ---
builder.Services.AddScoped<IValidator<UserModel>, UserValidator>();
builder.Services.AddScoped<IValidator<EventModel>, EventValidator>();

// --- HttpContext и фильтры ---
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped(typeof(ValidateModelAttribute<>));

// --- Session ---
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// --- Authorization ---
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("role", "Admin"));
});

// --- Logging ---
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

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

builder.Services.AddDbContext<EventContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Routing ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Event}/{action=Main}/{id?}");

app.MapRazorPages();

app.Run();
