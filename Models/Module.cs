using System;
using System.ComponentModel.DataAnnotations;

namespace UniManage.Models
{
    public class Module
    {
        [Key]
        public int ModuleID { get; set; }

        public string ModuleName { get; set; }

        public string ModuleCode { get; set; }

        public string Department { get; set; }

        public int Credits { get; set; }

        public string PdfPath { get; set; }

        public string PresentationPath { get; set; }

        public string VideoPath { get; set; }

        public string Description { get; set; }

        public int LecturerID { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}