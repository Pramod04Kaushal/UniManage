namespace UniManage.Models
{
    public class StudentDashboardViewModel
    {
        public List<UpcomingAssignmentViewModel> UpcomingAssignments { get; set; }

        public List<RecentGradeViewModel> RecentGrades { get; set; }
    }

    public class UpcomingAssignmentViewModel
    {
        public string Title { get; set; }
        public string CourseName { get; set; }
        public DateTime Deadline { get; set; }
    }

    public class RecentGradeViewModel
    {
        public string AssignmentTitle { get; set; }
        public decimal? Grade { get; set; }
        public bool IsPending { get; set; }
    }
}