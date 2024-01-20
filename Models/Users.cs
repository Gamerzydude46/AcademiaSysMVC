using Microsoft.AspNetCore.Identity;

namespace AcademiaSys.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
    }
}
