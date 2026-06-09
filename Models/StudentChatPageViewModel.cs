namespace UniManage.Models
{
    public class StudentChatPageViewModel
    {
        public List<LecturerMessageGroupViewModel> Groups
        { get; set; } = new();

        public List<LecturerChatViewModel> Lecturers
        { get; set; } = new();
    }
}