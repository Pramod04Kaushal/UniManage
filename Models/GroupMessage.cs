using System;

namespace UniManage.Models
{
    public class GroupMessage
    {
        public int GroupMessageID { get; set; }

        public int GroupID { get; set; }

        public int SenderUserID { get; set; }

        public string? MessageText { get; set; }

        public string? FilePath { get; set; }

        public string? FileName { get; set; }

        public DateTime SentAt { get; set; }
    }
}