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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


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
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 43))));

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
app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    int retries = 5;

    while (retries > 0)
    {
        try
        {
            Console.WriteLine("--- Tentando sincronizar tabelas no MySQL... ---");
            await context.Database.MigrateAsync(); Console.WriteLine("--- ✅ Tabelas verificadas/criadas com sucesso! ---");
            break;
        }
        catch (Exception ex)
        {
            retries--;
            Console.WriteLine($"--- ⚠️ Banco ainda não pronto. Tentativas restantes: {retries} ---");
            if (retries == 0) Console.WriteLine($"--- ❌ Erro final: {ex.Message} ---");
            await Task.Delay(5000);
        }
    }
}

app.Run();