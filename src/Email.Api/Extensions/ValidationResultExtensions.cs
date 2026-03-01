using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Email.Api.Extensions
{
    public static class ValidationResultExtensions
    {
        public static ValidationProblemDetails ToProblemDetails(this ValidationResult validationResult)
        {            
            return new ValidationProblemDetails(validationResult.ToDictionary())
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            };
        }
    }
}
