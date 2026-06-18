using System;
using System.ComponentModel.DataAnnotations;

namespace UniManage.Models
{
    public class CourseApplication
    {
        [Key]
        public int ApplicationID { get; set; }

        public int UserID { get; set; }

        public int CourseID { get; set; }

        public DateTime AppliedDate { get; set; }

        public string Status { get; set; }
    }
}