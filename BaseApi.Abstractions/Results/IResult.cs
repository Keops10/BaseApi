namespace BaseApi.Abstractions.Results;

public interface IResult
{
    bool IsSuccess { get; }
    string? Message { get; }
    string? ErrorCode { get; }
} 