using HobaBackend;
using HobaBackend.Auth;
using HobaBackend.DB;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<IAuthService, FirebaseAuthService>();
var configuration = builder.Configuration;
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(configuration.GetConnectionString("DbConnection"),
        x => x.MigrationsAssembly("HobaDB")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCustomAuth();

await app.RunAsync();