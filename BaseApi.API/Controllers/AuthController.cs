using BaseApi.Abstractions.Services;
using BaseApi.Abstractions.Results;
using BaseApi.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaseApi.API.Controllers.Base;

namespace BaseApi.API.Controllers;

[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var result = await _userService.RegisterAsync(registerDto);
        return CreateActionResult(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await _userService.LoginAsync(loginDto);
        return CreateActionResult(result);
    }

    [HttpGet("users")]
    [Authorize]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _userService.GetAllUsersAsync();
        return CreateActionResult(result);
    }

    [HttpGet("users/{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        return CreateActionResult(result);
    }

    [HttpPut("users/{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto)
    {
        var result = await _userService.UpdateUserAsync(id, updateUserDto);
        return CreateActionResult(result);
    }

    [HttpDelete("users/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await _userService.DeleteUserAsync(id);
        return CreateActionResult(result);
    }
} 