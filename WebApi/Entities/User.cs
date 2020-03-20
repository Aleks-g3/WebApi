

using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public string Role { get; set; }

        public double Credit { get; set; }
        public string Token { get; set; }
    }
}
