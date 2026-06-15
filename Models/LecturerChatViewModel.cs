namespace UniManage.Models
{
    public class LecturerChatViewModel
    {
        public int LecturerUserID { get; set; }

        public string LecturerName { get; set; } = "";

        public string Department { get; set; } = "";

        public int ChatID { get; set; }

        public int UnreadCount { get; set; }
    }
}