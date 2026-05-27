using System.Collections.Generic;

namespace UniManage.Models
{
    public class AddCourseViewModel
    {
        public string CourseName { get; set; }

        public string Department { get; set; }

        public int Semesters { get; set; }

        public string Duration { get; set; }

        public decimal CourseFee { get; set; }

        public string QualificationType { get; set; }

        public List<string> Intake { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public List<int> SelectedModules { get; set; }
    }
}