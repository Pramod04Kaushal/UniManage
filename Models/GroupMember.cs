using System.ComponentModel.DataAnnotations;

namespace UniManage.Models
{
    public class GroupMember
    {
        [Key]
        public int GroupMemberID { get; set; }

        public int GroupID { get; set; }

        public int UserID { get; set; }
    }
}