using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjectLab.PucCampinas.Common.Exceptions;

namespace ProjectLab.PucCampinas.Infrastructure.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        if (exception is AppException appEx)
        {
            httpContext.Response.StatusCode = appEx.StatusCode;
            problemDetails.Title = "Erro de Aplicação";
            problemDetails.Detail = appEx.Message;
            problemDetails.Status = appEx.StatusCode;
        }
        else
        {
            _logger.LogError(exception, "Erro inesperado");
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            problemDetails.Title = "Erro Interno do Servidor";
            problemDetails.Detail = "Ocorreu um erro inesperado. Contate o suporte.";
            problemDetails.Status = 500;
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}