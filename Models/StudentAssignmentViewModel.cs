namespace UniManage.Models
{
    public class StudentAssignmentViewModel
    {
        public int AssignmentID { get; set; }

        public string Title { get; set; }

        public string CourseName { get; set; }

        public string Description { get; set; }

        public DateTime Deadline { get; set; }

        public string? AttachmentPath { get; set; }

        public bool IsSubmitted { get; set; }

        public DateTime? SubmissionDate { get; set; }
    }
}