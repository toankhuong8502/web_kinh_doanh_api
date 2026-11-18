using System.Text.Json;
using WebKinhDoanh.Application.Common;

namespace WebKinhDoanh.Api.Infrastructure;

public sealed class ExceptionMiddleware(
    RequestDelegate next,
    ILogger<ExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (AppException exception)
        {
            logger.LogInformation(
                "Request {Method} {Path} failed with status {StatusCode}: {Message}",
                context.Request.Method,
                context.Request.Path,
                exception.StatusCode,
                exception.Message);
            await WriteErrorAsync(
                context,
                exception.StatusCode,
                ApiResponse<object?>.Fail(exception.Message, exception.Errors));
        }
        catch (Exception exception)
        {
            if (context.RequestAborted.IsCancellationRequested)
            {
                throw;
            }

            logger.LogError(
                exception,
                "Unhandled error while processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path);
            await WriteErrorAsync(
                context,
                StatusCodes.Status500InternalServerError,
                ApiResponse<object?>.Fail("Đã xảy ra lỗi hệ thống."));
        }
    }

    private static async Task WriteErrorAsync<T>(
        HttpContext context,
        int statusCode,
        ApiResponse<T> response)
    {
        if (context.Response.HasStarted)
        {
            throw new InvalidOperationException("Không thể ghi phản hồi lỗi sau khi response đã bắt đầu.");
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json; charset=utf-8";
        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, JsonOptions),
            context.RequestAborted);
    }
}
