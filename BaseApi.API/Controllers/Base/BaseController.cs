using BaseApi.Abstractions.Results;
using BaseApi.Application.Dtos;
using BaseApi.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace BaseApi.API.Controllers.Base;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected IActionResult CreateActionResult(Abstractions.Results.IResult result)
    {
        return result switch
        {
            SuccessResult successResult => Ok(new ApiResponse<object>
            {
                Success = true,
                Message = successResult.Message,
                Data = GetDataFromResult(successResult)
            }),
            Abstractions.Results.NotFoundResult => NotFound(new ApiResponse
            {
                Success = false,
                Message = result.Message,
                ErrorCode = result.ErrorCode
            }),
            InvalidResult => BadRequest(new ApiResponse
            {
                Success = false,
                Message = result.Message,
                ErrorCode = result.ErrorCode
            }),
            Abstractions.Results.UnauthorizedResult => Unauthorized(new ApiResponse
            {
                Success = false,
                Message = result.Message,
                ErrorCode = result.ErrorCode
            }),
            ForbiddenResult => Forbid(),
            Abstractions.Results.ConflictResult => Conflict(new ApiResponse
            {
                Success = false,
                Message = result.Message,
                ErrorCode = result.ErrorCode
            }),
            Abstractions.Results.NoContentResult => NoContent(),
            ErrorResult => StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = result.Message,
                ErrorCode = result.ErrorCode
            }),
            _ => StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = "Unknown error",
                ErrorCode = "UNKNOWN_ERROR"
            })
        };
    }

    protected virtual object? GetDataFromResult(Abstractions.Results.IResult result)
    {
        return result switch
        {
            SuccessResult<ProductDto> successResult => successResult.Data,
            SuccessResult<IEnumerable<ProductDto>> successResult => successResult.Data,
            SuccessResult<LoginResponseDto> successResult => successResult.Data,
            SuccessResult<UserDto> successResult => successResult.Data,
            SuccessResult<IEnumerable<UserDto>> successResult => successResult.Data,
            _ => null
        };
    }
} 