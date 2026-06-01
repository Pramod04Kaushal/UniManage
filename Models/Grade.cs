using System;
using System.ComponentModel.DataAnnotations;

namespace UniManage.Models
{
    public class Grade
    {
        [Key]
        public int GradeID { get; set; }

        public int SubmissionID { get; set; }

        public decimal Marks { get; set; }

        public string Feedback { get; set; }

        public int GradedBy { get; set; }

        public DateTime GradedDate { get; set; }
    }
}