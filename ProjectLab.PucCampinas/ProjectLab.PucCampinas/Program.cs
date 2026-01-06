using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjectLab.PucCampinas.Common.Services;
using ProjectLab.PucCampinas.Features.Laboratories.Service;
using ProjectLab.PucCampinas.Features.Reservations.Service;
using ProjectLab.PucCampinas.Features.Users.Service;
using ProjectLab.PucCampinas.Features.Users.Service.shared;
using ProjectLab.PucCampinas.Infrastructure.Data;
using ProjectLab.PucCampinas.Infrastructure.Email.Service;
using ProjectLab.PucCampinas.Infrastructure.Middleware;
using Serilog;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var loggerConfig = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console();

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

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT desta maneira: Bearer {seu token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
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

builder.Services.AddScoped<ProjectLab.PucCampinas.Features.Auth.Service.AuthService>();
builder.Services.AddScoped<ILaboratoryService, LaboratoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHttpClient<IViaCepService, ViaCepService>();

var jwtKey = builder.Configuration["JwtSettings:SecretKey"]; 
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

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

app.UseAuthentication();
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
            await context.Database.MigrateAsync();
            Console.WriteLine("--- ✅ Tabelas verificadas/criadas com sucesso! ---");
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