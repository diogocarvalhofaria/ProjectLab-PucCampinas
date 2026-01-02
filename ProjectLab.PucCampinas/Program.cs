using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProjectLab.PucCampinas.Common.Services;
using ProjectLab.PucCampinas.Features.Laboratories.Service;
using ProjectLab.PucCampinas.Features.Reservations.Service;
using ProjectLab.PucCampinas.Features.Users.Service;
using ProjectLab.PucCampinas.Features.Users.Service.shared;
using ProjectLab.PucCampinas.Infrastructure.Data;
using ProjectLab.PucCampinas.Infrastructure.Middleware;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

var loggerConfig = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console();

if (builder.Environment.IsProduction())
{
    var datadogKey = builder.Configuration["Datadog:ApiKey"];

    if (!string.IsNullOrEmpty(datadogKey))
    {
        loggerConfig.WriteTo.DatadogLogs(
            apiKey: datadogKey,
            source: "dotnet",
            service: "ProjectLabAPI",
            host: "http-intake.logs.datadoghq.com"
        );
    }
}

Log.Logger = loggerConfig.CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ProjectLab API",
        Version = "v1",
        Description = "API de Reservas de Laboratórios da PUC-Campinas"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddSingleton<ILoggerService, LoggerService>(); 
builder.Services.AddScoped<ICustomErrorHandler, CustomErrorHandler>(); 

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<ILaboratoryService, LaboratoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReservationService, ReservationService>();

builder.Services.AddHttpClient<IViaCepService, ViaCepService>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjectLab API");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();