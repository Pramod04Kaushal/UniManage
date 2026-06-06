using System.ComponentModel.DataAnnotations;

namespace UniManage.Models
{
    public class MessageGroup
    {
        [Key]
        public int GroupID { get; set; }

        public int CourseID { get; set; }

        public int BatchID { get; set; }

        public string GroupName { get; set; }
    }
}