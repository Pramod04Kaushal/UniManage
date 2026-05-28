using System.ComponentModel.DataAnnotations;

namespace UniManage.Models
{
    public class Lecturer
    {
        [Key]
        public int LecturerID { get; set; }

        public int UserID { get; set; }

        public string Department { get; set; }

        public string Specialization { get; set; }

        public string? Qualification { get; set; }

        public int? ExperienceYears { get; set; }
    }
}