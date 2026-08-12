using System.Net;
using TodoList.Application.Common.Responses;

namespace TodoList.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InvalidOperationException ex)
        {
            await WriteErrorResponseAsync(
                context,
                HttpStatusCode.BadRequest,
                ex.Message);
        }
        catch (Exception)
        {
            await WriteErrorResponseAsync(
                context,
                HttpStatusCode.InternalServerError,
                "Beklenmeyen bir hata oluştu.");
        }
    }

    private static async Task WriteErrorResponseAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string message)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = message
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}