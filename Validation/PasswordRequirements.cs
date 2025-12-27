using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TaskTrackerWeb.Validation
{
    public class PasswordRequirements : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var password = value as string;
            if (!Regex.IsMatch(password, "[0-9]") || !Regex.IsMatch(password, "(?=.*?[#?!@$%^&*-])") || !password.Any(char.IsUpper) || password.Length < 8) return new ValidationResult("Password does not match a requirements!");
            return ValidationResult.Success;
        }
    }
}