namespace BaseApi.Abstractions.Results;

public class ErrorResult : IResult
{
    public bool IsSuccess => false;
    public string? Message { get; }
    public string? ErrorCode { get; }

    public ErrorResult(string message, string? errorCode = null)
    {
        Message = message;
        ErrorCode = errorCode ?? "ERROR";
    }
}

public class ErrorResult<T> : ErrorResult
{
    public T? Data { get; }
    public ErrorResult(string message, string? errorCode = null) : base(message, errorCode) { }
} 