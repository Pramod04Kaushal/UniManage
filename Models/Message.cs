using System;
using System.ComponentModel.DataAnnotations;

namespace UniManage.Models
{
    public class Message
    {
        [Key]
        public int MessageID { get; set; }

        public int SenderID { get; set; }

        public int ReceiverID { get; set; }

        public string MessageText { get; set; }

        public DateTime SentAt { get; set; }

        public bool IsRead { get; set; }
    }
}