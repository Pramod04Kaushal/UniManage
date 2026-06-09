using System;
using System.ComponentModel.DataAnnotations;

namespace UniManage.Models
{
    public class PrivateChat
    {
        [Key]
        public int ChatID { get; set; }

        public int StudentUserID { get; set; }

        public int LecturerUserID { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}