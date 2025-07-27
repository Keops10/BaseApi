namespace BaseApi.Abstractions.Results;

public class ConflictResult : IResult
{
    public bool IsSuccess => false;
    public string? Message { get; }
    public string? ErrorCode => "CONFLICT";

    public ConflictResult(string? message = null)
    {
        Message = message ?? "Conflict";
    }
}

public class ConflictResult<T> : ConflictResult
{
    public T? Data { get; }
    public ConflictResult(string? message = null) : base(message) { }
} 