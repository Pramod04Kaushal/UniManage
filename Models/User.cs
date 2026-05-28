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

        public string? ProfileImage { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? Department { get; set; }

        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public string? Status { get; set; }


    }
}