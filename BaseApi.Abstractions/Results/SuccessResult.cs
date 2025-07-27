namespace BaseApi.Abstractions.Results;

public class SuccessResult : IResult
{
    public bool IsSuccess => true;
    public string? Message { get; }
    public string? ErrorCode => null;

    public SuccessResult(string? message = null)
    {
        Message = message;
    }
}

public class SuccessResult<T> : SuccessResult
{
    public T Data { get; }
    public SuccessResult(T data, string? message = null) : base(message)
    {
        Data = data;
    }
} 