using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ValidateModelAttribute<T> : ActionFilterAttribute
{
    private readonly IValidator<T> _validator;

    public ValidateModelAttribute(IValidator<T> validator)
    {
        _validator = validator;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ActionArguments.TryGetValue("model", out var model) && model is T)
        {
            var validationResult = _validator.Validate((T)model);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }
    }
}