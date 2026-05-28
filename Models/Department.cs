using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniManage.Models
{
    public class Department
    {
        [Key]
        public int DepartmentID { get; set; }

        public string DepartmentName { get; set; }


    }
}