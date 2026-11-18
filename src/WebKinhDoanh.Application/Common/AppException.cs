namespace WebKinhDoanh.Application.Common;

public sealed class AppException : Exception
{
    public AppException(int statusCode, string message, IDictionary<string, string[]>? errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }

    public int StatusCode { get; }
    public IDictionary<string, string[]>? Errors { get; }
}
