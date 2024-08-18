using System.Reflection;
using System.Text;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Api.Middlewares;
using Contract.Models;
using Domain.RepositoriyInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistance;
using Persistance.Repositories;
using Quartz;
using Services;
using Services.Abstractions;
using Services.Jobs;
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
IConfigurationSection appConfigSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appConfigSection);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web", Version = "v1" });

    // Define a custom header for Swagger requests
    c.AddSecurityDefinition("xAuth", new OpenApiSecurityScheme
    {
        Description = "Custom header for API authorization. Example: \"xAuth: {token}\"",
        Name = "xAuth",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "xAuth"
    });

    // Apply the custom header globally
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "xAuth"
                }
            },
            new string[] { }
        }
    });
});
    
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

IList<JobConfig> jobsConfig = builder.Configuration.GetSection("AppSettings:Quartz:JobConfig").Get<IList<JobConfig>>();

// Add Quartz services
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    foreach (var jobConfig in jobsConfig)
    {
        switch (jobConfig.JobName)
        {
            case "ResetUserJob":
                q.AddJob<ResetUserJob>(opts => opts.WithIdentity(jobConfig.JobName));
                break;
            case "SendEmailToResetPasswordJob":
                q.AddJob<SendEmailToResetPasswordJob>(opts => opts.WithIdentity(jobConfig.JobName));
                break;
            default:
                throw new ArgumentException($"Unknown job type: {jobConfig.JobName}");
        }
        
        q.AddTrigger(opts => opts
            .ForJob(jobConfig.JobName)
            .WithIdentity($"{jobConfig.JobName}-trigger")
            .WithCronSchedule(jobConfig.CronExpression));
    }
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

builder.Services.AddSingleton<IAmazonSimpleEmailService>(new AmazonSimpleEmailServiceClient(
    RegionEndpoint.APSoutheast1
));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.ConfigObject.AdditionalItems["headers"] = new
        {
            xAuth = new
            {
                Description = "Custom header for API authorization",
                Name = "xAuth",
                In = "header",
                Type = "string"
            }
        };
    });}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<AuthenticateMiddleware>();
app.Run();
