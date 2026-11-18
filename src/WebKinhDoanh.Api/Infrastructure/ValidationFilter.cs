using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebKinhDoanh.Application.Common;

namespace WebKinhDoanh.Api.Infrastructure;

public sealed class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var failures = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (context.HttpContext.RequestServices.GetService(validatorType) is not IValidator validator)
            {
                continue;
            }

            var validationContext = new ValidationContext<object>(argument);
            var result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);
            foreach (var error in result.Errors)
            {
                var key = JsonNamingPolicy.CamelCase.ConvertName(error.PropertyName);
                if (!failures.TryGetValue(key, out var messages))
                {
                    messages = [];
                    failures.Add(key, messages);
                }

                if (!messages.Contains(error.ErrorMessage, StringComparer.Ordinal))
                {
                    messages.Add(error.ErrorMessage);
                }
            }
        }

        if (failures.Count > 0)
        {
            context.Result = new BadRequestObjectResult(
                ApiResponse<object?>.Fail(
                    "Dữ liệu không hợp lệ.",
                    failures.ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value.ToArray(),
                        StringComparer.Ordinal)));
            return;
        }

        await next();
    }
}
