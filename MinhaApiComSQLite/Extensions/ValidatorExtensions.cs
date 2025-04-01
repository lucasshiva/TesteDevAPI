using ErrorOr;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MinhaApiComSQLite.Extensions;

public static class ValidatorExtensions
{
    public static void AddToModelState(
        this ValidationResult result,
        ModelStateDictionary modelState
    )
    {
        foreach (var error in result.Errors)
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
    }

    public static void AddToModelState(this List<Error> errors, ModelStateDictionary modelState)
    {
        foreach (var error in errors)
            modelState.AddModelError(error.Code, error.Description);
    }
}
