using System;
using System.ComponentModel.DataAnnotations;

namespace UniManage.Models
{
    public class User
    {

        [Key]
        public int UserID { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public string Role { get; set; }

        public string Phone { get; set; }

        public string ProfileImage { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}