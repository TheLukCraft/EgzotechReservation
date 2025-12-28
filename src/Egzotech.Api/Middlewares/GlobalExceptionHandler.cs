using System.Net;
using System.Text.Json;

namespace Egzotech.Api.Middlewares;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = exception switch
        {
            // 409
            InvalidOperationException => (int)HttpStatusCode.Conflict, 
            
            // 404
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            
            // 400
            ArgumentException => (int)HttpStatusCode.BadRequest,
            
            // 500
            _ => (int)HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = statusCode;

        var response = new
        {
            StatusCode = statusCode,
            Message = exception.Message,
            Detailed = statusCode == 500 ? "Internal Server Error" : exception.Message 
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}