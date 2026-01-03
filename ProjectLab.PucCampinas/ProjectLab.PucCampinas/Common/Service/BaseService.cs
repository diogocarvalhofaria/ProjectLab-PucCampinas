namespace ProjectLab.PucCampinas.Common.Services;

public abstract class BaseService
{
    private readonly ICustomErrorHandler _errorHandler;

    protected BaseService(ICustomErrorHandler errorHandler)
    {
        _errorHandler = errorHandler;
    }

    protected void OnError(object trace, int statusCode = 400)
    {
        _errorHandler.OnError(statusCode, trace);
    }
}