using System.ComponentModel.DataAnnotations;

namespace UniManage.Models
{
    public class Student
    {
        [Key]
        public int StudentID { get; set; }

        public int UserID { get; set; }

        public string RegNum { get; set; }

        public string Department { get; set; }

        public int Semester { get; set; }
    }
}