namespace ApiRefactor.Models
{
    public class LoginResponse
    {
        public required string Email { get; set; }
        public required string Token { get; set; }
        public required string Role { get; set; }
    }
}
