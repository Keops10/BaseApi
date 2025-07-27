using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using BaseApi.API.Models;

namespace BaseApi.API.Filters;

public class ValidationFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            var response = new ApiResponse
            {
                Success = false,
                Message = "Validation failed",
                ErrorCode = "VALIDATION_ERROR"
            };

            context.Result = new BadRequestObjectResult(response);
        }
    }
} 