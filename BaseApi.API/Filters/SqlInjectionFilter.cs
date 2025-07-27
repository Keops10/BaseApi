using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.RegularExpressions;

namespace BaseApi.API.Filters;

public class SqlInjectionFilter : ActionFilterAttribute
{
    private static readonly string[] SqlKeywords = {
        "SELECT", "INSERT", "UPDATE", "DELETE", "DROP", "CREATE", "ALTER", "EXEC", "EXECUTE",
        "UNION", "OR", "AND", "WHERE", "FROM", "JOIN", "HAVING", "GROUP BY", "ORDER BY"
    };

    private static readonly string[] SqlPatterns = {
        @"(\b(SELECT|INSERT|UPDATE|DELETE|DROP|CREATE|ALTER|EXEC|EXECUTE)\b)",
        @"(\b(UNION|OR|AND)\b)",
        @"(--|\/\*|\*\/)",
        @"(xp_cmdshell|sp_executesql)",
        @"(WAITFOR|DELAY|BENCHMARK)",
        @"(INTO\s+OUTFILE|DUMPFILE)",
        @"(LOAD_FILE|UPDATEXML|EXTRACTVALUE)"
    };

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var parameter in context.ActionArguments)
        {
            if (parameter.Value is string stringValue)
            {
                if (ContainsSqlInjection(stringValue))
                {
                    var response = new
                    {
                        Success = false,
                        Message = "Potentially malicious input detected",
                        ErrorCode = "SECURITY_VIOLATION"
                    };

                    context.Result = new BadRequestObjectResult(response);
                    return;
                }
            }
        }
    }

    private bool ContainsSqlInjection(string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;

        var upperInput = input.ToUpperInvariant();

        // Check for SQL keywords
        foreach (var keyword in SqlKeywords)
        {
            if (upperInput.Contains(keyword))
            {
                // Check if it's part of a larger word (false positive)
                var pattern = $@"\b{Regex.Escape(keyword)}\b";
                if (Regex.IsMatch(upperInput, pattern))
                {
                    return true;
                }
            }
        }

        // Check for SQL patterns
        foreach (var pattern in SqlPatterns)
        {
            if (Regex.IsMatch(upperInput, pattern, RegexOptions.IgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
} 