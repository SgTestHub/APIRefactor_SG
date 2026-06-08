using ApiRefactor.Configurations;
using System.ComponentModel.DataAnnotations;

namespace ApiRefactor.Models
{
    public class LocalUser
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }

        public required string Email { get; set; }
        public required string Password { get; set; }
        public string Role { get; set; } = ApplicationConstants.Role_Customer;

    }
}
