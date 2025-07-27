namespace BaseApi.Abstractions.Results;

public class ForbiddenResult : IResult
{
    public bool IsSuccess => false;
    public string? Message { get; }
    public string? ErrorCode => "FORBIDDEN";

    public ForbiddenResult(string? message = null)
    {
        Message = message ?? "Forbidden";
    }
}

public class ForbiddenResult<T> : ForbiddenResult
{
    public T? Data { get; }
    public ForbiddenResult(string? message = null) : base(message) { }
} 