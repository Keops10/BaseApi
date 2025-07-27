namespace BaseApi.Abstractions.Results;

public class UnauthorizedResult : IResult
{
    public bool IsSuccess => false;
    public string? Message { get; }
    public string? ErrorCode => "UNAUTHORIZED";

    public UnauthorizedResult(string? message = null)
    {
        Message = message ?? "Unauthorized";
    }
}

public class UnauthorizedResult<T> : UnauthorizedResult
{
    public T? Data { get; }
    public UnauthorizedResult(string? message = null) : base(message) { }
} 