using System;

namespace UniManage.Models
{
    public class AddStudentViewModel
    {
        
        public string FullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string Department { get; set; }

        public string Course { get; set; }

        public int Semester { get; set; }

        public int EnrollmentYear { get; set; }

        public string Status { get; set; }
    }
}