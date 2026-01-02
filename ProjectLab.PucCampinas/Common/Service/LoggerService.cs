using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ProjectLab.PucCampinas.Common.Services;

public interface ILoggerService
{
    void Log(object message);
    void Info(object message);
    void Error(object trace);
    void Warn(string origin, string dt = "");
}

public class LoggerService : ILoggerService
{
    public void Log(object message)
    {
        var msg = message is string ? message.ToString() : JsonSerializer.Serialize(message);
        Console.WriteLine(msg);
    }

    public void Info(object message)
    {
        var msg = message is string ? message.ToString() : JsonSerializer.Serialize(message);
        Console.WriteLine(msg);

        Serilog.Log.Information(msg);
    }

    public void Warn(string origin, string dt = "")
    {
        Serilog.Log.Warning("{Origin} => {Dt}", origin, dt);
    }

    public void Error(object trace)
    {
        Console.WriteLine(trace);

        if (trace is DbUpdateException dbEx)
        {
            Serilog.Log.Error(dbEx, "Erro ao salvar entidade: => {Message}", dbEx.InnerException?.Message ?? dbEx.Message);
        }
        else if (trace is HttpRequestException httpEx)
        {
            Serilog.Log.Error(httpEx, "MESSAGE => {Message} StatusCode => {Status}", httpEx.Message, httpEx.StatusCode);
        }
        else if (trace is JsonException jsonEx)
        {
            Serilog.Log.Error(jsonEx, "Erro de JSON Parse: {Stack}", jsonEx.StackTrace);
        }
        else if (trace is Exception ex)
        {
            Serilog.Log.Error(ex, "{Stack}", ex.StackTrace);
        }
        else if (trace is string str)
        {
            Serilog.Log.Error(str);
        }
        else
        {
            Serilog.Log.Error("Erro genérico: {Trace}", JsonSerializer.Serialize(trace));
        }
    }
}