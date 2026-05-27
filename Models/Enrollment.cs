using System;
using System.ComponentModel.DataAnnotations;

namespace UniManage.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentID { get; set; }

        public int StudentID { get; set; }

        public int CourseID { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public string Status { get; set; }
    }
}