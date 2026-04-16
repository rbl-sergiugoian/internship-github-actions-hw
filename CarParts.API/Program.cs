using CarParts.API.Middleware.Auth;
using CarParts.API.Middleware.Authentication;
using CarParts.Common.Authorization;
using CarParts.DataPersistence.Repositories;
using CarParts.Services.Auth;
using CarParts.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ICarPartsRepository, CarPartsRepository>();
builder.Services.AddScoped<ICarPartsService, CarPartsService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/test", () => "test");

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

if (app.Environment.IsDevelopment())
{
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
