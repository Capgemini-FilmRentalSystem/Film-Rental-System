using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FilmRentalStore.API.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var errors = new Dictionary<string, string[]>();

            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument == null)
                    continue;

                var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());

                if (context.HttpContext.RequestServices.GetService(validatorType) is not IValidator validator)
                    continue;

                var validationContext = new ValidationContext<object>(argument);

                var result = await validator.ValidateAsync(validationContext);

                if (!result.IsValid)
                {
                    foreach (var errorGroup in result.Errors.GroupBy(e => e.PropertyName))
                    {
                        errors[errorGroup.Key] = errorGroup
                            .Select(e => e.ErrorMessage)
                            .ToArray();
                    }
                }
            }

            if (errors.Any())
            {
                context.Result = new BadRequestObjectResult(new
                {
                    success = false,
                    message = "Validation failed.",
                    errors
                });

                return;
            }

            await next();
        }
    }
}