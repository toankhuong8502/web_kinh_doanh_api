namespace WebKinhDoanh.Application.Common;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public required string Message { get; init; }
    public T? Data { get; init; }
    public IDictionary<string, string[]>? Errors { get; init; }

    public static ApiResponse<T> Ok(T? data, string message = "Thành công") =>
        new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> Fail(
        string message,
        IDictionary<string, string[]>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors };
}
