using System.ComponentModel.DataAnnotations;
using TaskTrackerWeb.Validation;

namespace TaskTrackerWeb.Models
{
    public class RegisterModel
    {
        [Required, Length(1, 100)]
        public string Name { get; set; } = string.Empty;
        [Required, Length(1, 100)]
        public string Surname { get; set; } = string.Empty;
        [Required, Length(1, 100), EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, PasswordRequirements]
        public string Password { get; set; } = string.Empty;
        [Compare(nameof(Password), ErrorMessage = "Password does not match!")]
        public string ConfirmedPassword { get; set; } = string.Empty;
    }
}
