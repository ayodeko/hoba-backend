using HobaBackend;
using HobaBackend.Auth;
using HobaBackend.Auth.Utilities;
using HobaBackend.DB;
using HobaBackend.DB.Repositories;
using HobaBackend.Endpoints;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var conString = builder.Configuration.GetConnectionString("NeonConnection");
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IAuthService, FirebaseAuthService>();
builder.Services.AddSingleton<IPasswordGenerator, PasswordGenerator>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseNpgsql(conString),
    contextLifetime: ServiceLifetime.Singleton);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    await app.ApplyMigrations();
}

app.UseHttpsRedirection();
app.UseCustomAuth();

app.MapUserEndpoints();

await app.RunAsync();