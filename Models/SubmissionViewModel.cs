namespace UniManage.Models
{
    public class SubmissionViewModel
    {
        public int SubmissionID { get; set; }

        public string StudentName { get; set; }

        public string RegNum { get; set; }

        public string AssignmentTitle { get; set; }

        public DateTime SubmissionDate { get; set; }

        public string Status { get; set; }

        public string FilePath { get; set; }

        public decimal? Grade { get; set; }
    }
}