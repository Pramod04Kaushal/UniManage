using System;

namespace UniManage.Models
{
    public class AddLecturerViewModel
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string Department { get; set; }

        public string Specialization { get; set; }

        public string Qualification { get; set; }

        public int ExperienceYears { get; set; }

        public string Status { get; set; }
    }
}