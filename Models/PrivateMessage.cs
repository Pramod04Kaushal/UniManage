using System;
using System.ComponentModel.DataAnnotations;

namespace UniManage.Models
{
    public class PrivateMessage
    {
        [Key]
        public int MessageID { get; set; }

        public int ChatID { get; set; }

        public int SenderUserID { get; set; }

        public string? MessageText { get; set; }

        public string? FilePath { get; set; }

        public string? FileName { get; set; }

        public DateTime SentAt { get; set; }

        public bool IsRead { get; set; } = false;
    }
}