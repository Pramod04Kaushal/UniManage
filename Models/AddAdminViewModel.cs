using System;
using Microsoft.AspNetCore.Http;

namespace UniManage.Models
{
    public class AddAdminViewModel
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string Department { get; set; }

        public string Position { get; set; }

        public string OfficeLocation { get; set; }

        public string AccessLevel { get; set; }

        public string Status { get; set; }

        public int UserID { get; set; }

        public IFormFile? ProfileImageFile { get; set; }

        public string? ProfileImage { get; set; }
    }
}