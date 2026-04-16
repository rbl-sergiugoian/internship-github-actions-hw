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
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AuthorizationSettings>(
    builder.Configuration.GetSection("AuthorizationSettings"));

var authSettings = builder.Configuration
    .GetSection("AuthorizationSettings")
    .Get<AuthorizationSettings>();

builder.Services.AddSingleton<ICarPartsRepository, CarPartsRepository>();
builder.Services.AddScoped<ICarPartsService, CarPartsService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IJwtUtils, JwtUtils>();

builder.Services.AddControllers();
builder.Services.AddProblemDetails();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = authSettings.Issuer,
                    ValidAudience = authSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(authSettings.Secret)
                    ),
                    ClockSkew = TimeSpan.Zero
                };
            });


builder.Services.AddAuthorization(config =>
{
    config.AddPolicy(Policies.User, policy =>
        policy.Requirements.Add(new PrivilegeRequirement(Policies.User)));

    config.AddPolicy(Policies.Admin, policy =>
        policy.Requirements.Add(new PrivilegeRequirement(Policies.Admin)));

    config.AddPolicy(Policies.All, policy =>
        policy.Requirements.Add(new PrivilegeRequirement(Policies.All)));
});

builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Car Parts App",
        Description = "Simple web API that helps usrs manage their cae parts",
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter only the JWT token"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });

    app.UseExceptionHandler(new ExceptionHandlerOptions
    {
        StatusCodeSelector = exception => exception switch 
        {
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            ArgumentException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        }
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();
