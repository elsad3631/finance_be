using BackEnd.Entities;

namespace BackEnd.Models.AuthModels
{
    public class LoginResponse
    {
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
