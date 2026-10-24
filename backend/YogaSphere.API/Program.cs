using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.Win32;
using System.Text;
using YogaSphere.Application;
using YogaSphere.Application.Interfaces;
using YogaSphere.Application.Interfaces.Repositories;
using YogaSphere.Infrastructure.Persistence;
using YogaSphere.Infrastructure.Repositories;
using YogaSphere.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication(); //registers application services

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

//Add standard swagger generator


builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT access token."
        });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});



builder.Services.AddDbContext<YogaSphereDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("YogaSphereConn")));

//Register repositories and services

//It creates one repository instance per HTTP request, which fits the scoped lifetime of YogaSphereDbContext
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ICustomerProfileRepository, CustomerProfileRepository>();

//Register customer profile business logic.
builder.Services.AddScoped<ICustomerService, CustomerService>();


// Configure JWT authentication
var jwtSection = builder.Configuration.GetSection("Jwt");

var issuer = jwtSection["Issuer"];
var audience = jwtSection["Audience"];
var secretKey = jwtSection["SecretKey"];

if (string.IsNullOrWhiteSpace(issuer) ||
    string.IsNullOrWhiteSpace(audience) ||
    string.IsNullOrWhiteSpace(secretKey) ||
    Encoding.UTF8.GetByteCount(secretKey) < 32)
{
    throw new InvalidOperationException(
        "JWT configuration is missing or invalid.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Reject tokens issued by an unexpected issuer or intended for another audience.
            ValidateIssuer = true,
            ValidIssuer = issuer,

            ValidateAudience = true,
            ValidAudience = audience,

            // Ensure the token was signed with our trusted secret key.
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey)),

            // Reject expired tokens; allow only a small clock difference between servers.
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),

            NameClaimType = System.Security.Claims.ClaimTypes.Name,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger(); //generates swagger.json file
    app.UseSwaggerUI(
        c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "YogaSphere.API v1")); //specifies the swagger.json file endpoint
}

app.UseHttpsRedirection();

// Authenticate the request before checking its permissions.
app.UseAuthentication();

// Enforce [Authorize] and role-based access restrictions.s
app.UseAuthorization();

app.MapControllers();

app.Run();
