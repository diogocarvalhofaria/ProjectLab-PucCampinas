using ProjectLab.PucCampinas.Common.Exceptions;
namespace ProjectLab.PucCampinas.Common.Services;

public interface ICustomErrorHandler
{
    void OnError(int status, object trace, bool rethrow = true);
}

public class CustomErrorHandler : ICustomErrorHandler
{
    private readonly ILoggerService _loggerService;

    public CustomErrorHandler(ILoggerService loggerService)
    {
        _loggerService = loggerService;
    }

    public void OnError(int status, object trace, bool rethrow = true)
    {
        _loggerService.Error(trace);

        if (rethrow)
        {
            string message = trace is Exception ex ? ex.Message : trace.ToString() ?? "Erro desconhecido";

            throw new AppException(message, status);
        }
    }
}