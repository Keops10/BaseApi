using BaseApi.Abstractions.Repositories;
using BaseApi.Abstractions.Results;
using BaseApi.Abstractions.Services;

using BaseApi.Application.Dtos;
using BaseApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Microsoft.Extensions.Configuration;

namespace BaseApi.Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserService> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public UserService(UserManager<ApplicationUser> userManager, ILogger<UserService> logger, IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _userManager = userManager;
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<IResult> RegisterAsync(object registerDtoObj)
    {
        try
        {
            var registerDto = (RegisterDto)registerDtoObj;

            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                return new InvalidResult("Şifreler uyuşmuyor", "PASSWORD_MISMATCH");
            }

            if (await _userManager.FindByNameAsync(registerDto.UserName) != null)
            {
                return new ConflictResult("Kullanıcı adı zaten mevcut");
            }

            if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
            {
                return new ConflictResult("Email zaten mevcut");
            }

            var user = _mapper.Map<ApplicationUser>(registerDto);
            _logger.LogInformation("Kullanıcı oluşturuluyor: {UserName}", registerDto.UserName);
            
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Kullanıcı oluşturma başarısız: {Errors}", errors);
                return new ErrorResult(errors, "REGISTRATION_FAILED");
            }

            _logger.LogInformation("Kullanıcı başarıyla oluşturuldu: {UserId}", user.Id);
            
            // Kullanıcının gerçekten kaydedildiğini doğrula
            var savedUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (savedUser == null)
            {
                _logger.LogError("Kullanıcı veritabanına kaydedilemedi: {UserId}", user.Id);
                return new ErrorResult("Kullanıcı veritabanına kaydedilemedi", "DATABASE_ERROR");
            }
            
            _logger.LogInformation("Kullanıcı veritabanında doğrulandı: {UserId}", user.Id);

            var userDto = _mapper.Map<UserDto>(savedUser);
            return new SuccessResult<UserDto>(userDto, "Kullanıcı başarıyla kaydedildi");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Kayıt işlemi sırasında hata oluştu: {ex.Message}");
        }
    }

    public async Task<IResult> LoginAsync(object loginDtoObj)
    {
        try
        {
            var loginDto = (LoginDto)loginDtoObj;
            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user == null)
            {
                return new NotFoundResult<UserDto>("Kullanıcı bulunamadı");
            }

            if (!user.IsActive)
            {
                return new ForbiddenResult("Kullanıcı pasif durumda");
            }

            var isValidPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isValidPassword)
            {
                return new UnauthorizedResult("Kullanıcı adı veya şifre hatalı");
            }

            // Son giriş tarihini güncelle
            user.LastLoginDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var token = await GenerateJwtTokenAsync(user);

            var loginResponse = new LoginResponseDto
            {
                Token = token,
                User = _mapper.Map<UserDto>(user)
            };

            return new SuccessResult<LoginResponseDto>(loginResponse, "Giriş başarılı");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Giriş işlemi sırasında hata oluştu: {ex.Message}");
        }
    }

    public async Task<IResult> GetUserByIdAsync(Guid id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new NotFoundResult<UserDto>("Kullanıcı bulunamadı");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return new SuccessResult<UserDto>(userDto);
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Kullanıcı getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<IResult> GetUserByUsernameAsync(string username)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return new NotFoundResult<UserDto>("Kullanıcı bulunamadı");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return new SuccessResult<UserDto>(userDto);
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Kullanıcı getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<IResult> GetAllUsersAsync()
    {
        try
        {
            // Yeni generic repository pattern kullanımı
            var users = await _unitOfWork.GetRepository<ApplicationUser>().GetAllAsync();
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
            return new SuccessResult<IEnumerable<UserDto>>(userDtos);
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Kullanıcılar getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<IResult> UpdateUserAsync(Guid id, object updateUserDtoObj)
    {
        try
        {
            var updateUserDto = (UpdateUserDto)updateUserDtoObj;
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return new NotFoundResult<UserDto>("Kullanıcı bulunamadı");
            }

            // AutoMapper ile güncelle
            _mapper.Map(updateUserDto, user);
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = "system";

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new ErrorResult(errors, "UPDATE_FAILED");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return new SuccessResult<UserDto>(userDto, "Kullanıcı güncellendi");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Kullanıcı güncellenirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<IResult> DeleteUserAsync(Guid id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new NotFoundResult("Kullanıcı bulunamadı");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new ErrorResult(errors, "DELETE_FAILED");
            }

            return new NoContentResult("Kullanıcı silindi");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Kullanıcı silinirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<string> GenerateJwtTokenAsync(object userObj)
    {
        var user = (ApplicationUser)userObj;
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var secretKey = _configuration["JwtSettings:SecretKey"] ?? "your-super-secret-key-with-at-least-32-characters";
        var issuer = _configuration["JwtSettings:Issuer"] ?? "BaseApi";
        var audience = _configuration["JwtSettings:Audience"] ?? "BaseApi";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return $"Bearer {tokenString}";
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            // Bearer prefix'ini kaldır
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring("Bearer ".Length);
            }

            var secretKey = _configuration["JwtSettings:SecretKey"] ?? "your-super-secret-key-with-at-least-32-characters";
            var issuer = _configuration["JwtSettings:Issuer"] ?? "BaseApi";
            var audience = _configuration["JwtSettings:Audience"] ?? "BaseApi";

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }
} 