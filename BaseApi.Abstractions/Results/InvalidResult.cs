namespace BaseApi.Abstractions.Results;

public class InvalidResult : IResult
{
    public bool IsSuccess => false;
    public string? Message { get; }
    public string? ErrorCode { get; }

    public InvalidResult(string message, string? errorCode = null)
    {
        Message = message;
        ErrorCode = errorCode ?? "INVALID_REQUEST";
    }
}

public class InvalidResult<T> : InvalidResult
{
    public T? Data { get; }
    public InvalidResult(string message, string? errorCode = null) : base(message, errorCode) { }
} 