using System;
using System.ComponentModel.DataAnnotations;

namespace UniManage.Models
{
    public class Course
    {
        [Key]
        public int CourseID { get; set; }

        public string CourseCode { get; set; }

        public string CourseName { get; set; }

        public string Department { get; set; }

        public int Semesters { get; set; }

        public string Duration { get; set; }

        public decimal CourseFee { get; set; }

        public string QualificationType { get; set; }

        public string Intake { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}