using System;
using System.ComponentModel.DataAnnotations;

namespace SurfUpRedux.Attributes
{
    public class IgnoreOnFormSubmitAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            return ValidationResult.Success;
        }
    }
}
