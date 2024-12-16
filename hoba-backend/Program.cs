using HobaBackend;
using HobaBackend.Auth;
using HobaBackend.DB;
using HobaBackend.Endpoints;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => { loggerConfig.ReadFrom.Configuration(context.Configuration); });

builder.Services.AddOpenApi();
builder.Services.AddAuthServices(builder.Configuration);
builder.Services.AddDbServices(builder.Configuration);
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.ApplyMigrations();
}

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();
//app.UseCustomAuth();

app.MapUserEndpoints();

// Redirect to Scalar OpenAPI docs
app.MapGet("/", () => Results.LocalRedirect("/scalar/v1"))
    .ExcludeFromDescription();

await app.RunAsync();