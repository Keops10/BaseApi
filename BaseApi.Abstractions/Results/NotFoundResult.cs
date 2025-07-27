namespace BaseApi.Abstractions.Results;

public class NotFoundResult : IResult
{
    public bool IsSuccess => false;
    public string? Message { get; }
    public string? ErrorCode => "NOT_FOUND";

    public NotFoundResult(string? message = null)
    {
        Message = message ?? "Entity not found";
    }
}

public class NotFoundResult<T> : NotFoundResult
{
    public T? Data { get; }
    public NotFoundResult(string? message = null) : base(message) { }
} 