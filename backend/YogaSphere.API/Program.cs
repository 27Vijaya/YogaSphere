using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using YogaSphere.Application;
using YogaSphere.Application.Interfaces;
using YogaSphere.Application.Interfaces.Repositories;
using YogaSphere.Infrastructure.Configuration;
using YogaSphere.Infrastructure.Persistence;
using YogaSphere.Infrastructure.Repositories;
using YogaSphere.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);


// Bind Razorpay credentials from configuration and User Secrets.
builder.Services.Configure<RazorpaySettings>(
    builder.Configuration.GetSection("Razorpay"));

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
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddScoped<ITrainerProfileRepository, TrainerProfileRepository>();
builder.Services.AddScoped<ITrainerService, TrainerService>();

builder.Services.AddScoped<IYogaProgramRepository, YogaProgramRepository>();
builder.Services.AddScoped<IYogaProgramService, YogaProgramService>();

builder.Services.AddScoped<IYogaClassRepository, YogaClassRepository>();
builder.Services.AddScoped<IYogaClassService, YogaClassService>();

builder.Services.AddScoped<IScheduleRepository,ScheduleRepository>();
builder.Services.AddScoped<IScheduleService,ScheduleService>();

builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IWebhookEventRepository, WebhookEventRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Register Razorpay integration with HttpClient.
builder.Services.AddHttpClient<IRazorpayService, RazorpayService>();
builder.Services.AddScoped<IRazorpayWebhookService, RazorpayWebhookService>();


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


// Configure CORS to allow requests from the React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Enable CORS for the React frontend
app.UseCors("ReactFrontend");

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
