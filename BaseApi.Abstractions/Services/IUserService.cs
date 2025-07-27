using BaseApi.Abstractions.Results;

namespace BaseApi.Abstractions.Services;

public interface IUserService
{
    Task<IResult> RegisterAsync(object registerDto);
    Task<IResult> LoginAsync(object loginDto);
    Task<IResult> GetUserByIdAsync(Guid id);
    Task<IResult> GetUserByUsernameAsync(string username);
    Task<IResult> GetAllUsersAsync();
    Task<IResult> UpdateUserAsync(Guid id, object updateUserDto);
    Task<IResult> DeleteUserAsync(Guid id);
    Task<string> GenerateJwtTokenAsync(object user);
    Task<bool> ValidateTokenAsync(string token);
} 