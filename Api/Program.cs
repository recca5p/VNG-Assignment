using Api.Middlewares;
using Domain.RepositoriyInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Persistance;
using Persistance.Repositories;
using Services;
using Services.Abstractions;
using Services.Profiles;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .Build();

String envInUsed = configuration.GetSection("ASPNETCORE_ENVIRONMENT").Value 
                   ?? throw new InvalidOperationException("Can not get ASPNETCORE_ENVIRONMENT value");

WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    EnvironmentName = envInUsed
});
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web", Version = "v1" }));
    
builder.Services.AddScoped<IServiceManager, ServiceManager>();
    
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
    
string connectionString = builder.Configuration.GetSection("AppSettings:DatabaseConnectionString").Value 
                          ?? throw new InvalidOperationException("Can not get DatabaseConnectionString");

builder.Services.AddScoped<IRepositoryDbContext>(x =>
    new RepositoryDbContext(
        new DbContextOptionsBuilder<RepositoryDbContext>()
            .UseSqlServer(connectionString)
            .Options));


builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services.AddAutoMapper(typeof(ModelProfile));
builder.Services.AddAutoMapper(typeof(RequestProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();
