using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace hospital_management.Filters;

public class ValidationActionFilter : IAsyncActionFilter
{
    private readonly ILogger<ValidationActionFilter> _logger;
    public ValidationActionFilter(ILogger<ValidationActionFilter> logger)
    {
        _logger = logger;
    }
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            _logger.LogWarning("Validation failed: {ModelState}", context.ModelState);
            context.Result = new BadRequestObjectResult(context.ModelState);
            return;
        }
        _logger.LogInformation("Validation passed");
        await next();
    }
}