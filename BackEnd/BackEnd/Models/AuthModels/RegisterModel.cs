using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.AuthModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public string? Role { get; set; } = null;
    }
}
