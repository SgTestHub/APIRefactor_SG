using System.ComponentModel.DataAnnotations;

namespace ApiRefactor.Models
{
    public class Login
    {
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }   
}
