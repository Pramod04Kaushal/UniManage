using System;
using System.ComponentModel.DataAnnotations;

namespace UniManage.Models
{
    public class AssignmentSubmission
    {
        [Key]
        public int SubmissionID { get; set; }

        public int AssignmentID { get; set; }

        public int StudentID { get; set; }

        public string FilePath { get; set; }

        public DateTime SubmissionDate { get; set; }

        public string Status { get; set; }

        public decimal? Grade { get; set; }
    }
}