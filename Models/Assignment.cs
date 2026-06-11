using System;
using System.ComponentModel.DataAnnotations;

namespace UniManage.Models
{
    public class Assignment
    {
        [Key]
        public int AssignmentID { get; set; }

        public int CourseID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime Deadline { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? LecturerID { get; set; }

        public int? Semester { get; set; }

        public int? BatchID { get; set; }

        public string? AttachmentPath { get; set; }
        public int? ModuleID { get; set; }
    }
}