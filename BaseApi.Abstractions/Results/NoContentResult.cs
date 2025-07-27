namespace BaseApi.Abstractions.Results;

public class NoContentResult : IResult
{
    public bool IsSuccess => true;
    public string? Message { get; }
    public string? ErrorCode => null;

    public NoContentResult(string? message = null)
    {
        Message = message;
    }
}

public class NoContentResult<T> : NoContentResult
{
    public T? Data { get; }
    public NoContentResult(string? message = null) : base(message) { }
} 