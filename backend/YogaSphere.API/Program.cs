using Microsoft.EntityFrameworkCore;
using YogaSphere.Infrastructure.Persistence;
using YogaSphere.Application.Interfaces.Repositories;
using YogaSphere.Infrastructure.Repositories;
using YogaSphere.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication(); //registers application services

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

//Add standard swagger generator
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<YogaSphereDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("YogaSphereConn")));

//It creates one repository instance per HTTP request, which fits the scoped lifetime of YogaSphereDbContext
builder.Services.AddScoped<IUserRepository, UserRepository>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
