using System.Diagnostics.CodeAnalysis;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SFA.DAS.Admin.Aan.Web.Extensions;

[ExcludeFromCodeCoverage]
public static class ModelStateExtensions
{
    public static void AddValidationErrors(this ModelStateDictionary modelState, IEnumerable<ValidationFailure> errors)
    {
        foreach (var error in errors)
        {
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
    }
}
