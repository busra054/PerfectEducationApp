namespace WebApplication_Domain.Entities
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public bool RememberMe { get; set; }
    }
}
