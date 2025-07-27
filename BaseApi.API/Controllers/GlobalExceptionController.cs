using Microsoft.AspNetCore.Mvc;
using BaseApi.Domain.Exceptions;
using BaseApi.Application.Exceptions;
using BaseApi.API.Controllers.Base;

namespace BaseApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GlobalExceptionController : BaseController
{
    [HttpGet("test-exception")]
    public IActionResult TestException()
    {
        throw new BusinessException("Bu bir test exception'ıdır");
    }

    [HttpGet("test-validation")]
    public IActionResult TestValidation()
    {
        throw new ValidationException("Validation hatası test");
    }
} 