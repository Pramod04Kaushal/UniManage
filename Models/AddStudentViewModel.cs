using System;
using Microsoft.AspNetCore.Http;

namespace UniManage.Models
{
    public class AddStudentViewModel
    {
        public IFormFile? ProfileImageFile { get; set; }
        public int UserID { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string Department { get; set; }

        public string Course { get; set; }

        public int Semester { get; set; }

        public int? EnrollmentYear { get; set; }

        public string Status { get; set; }

        public string? ProfileImage { get; set; }

        public int? BatchID { get; set; }
    }
}