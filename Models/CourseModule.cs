using System.ComponentModel.DataAnnotations;

namespace UniManage.Models
{
    public class CourseModule
    {
        [Key]
        public int CourseModuleID { get; set; }

        public int CourseID { get; set; }

        public int ModuleID { get; set; }

        public int Semester { get; set; }
    }
}