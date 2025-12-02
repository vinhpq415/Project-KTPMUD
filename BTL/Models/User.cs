using System.ComponentModel.DataAnnotations;

namespace BTL.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "Admin" (Quản lý), "Guard" (Bảo vệ), "Customer" (Khách)
    }
}