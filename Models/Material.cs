using System.ComponentModel.DataAnnotations;

namespace UniManage.Models
{
    public class Material
    {
        [Key]
        public int MaterialID { get; set; }

        public int ModuleID { get; set; }

        public string Title { get; set; }

        public string MaterialType { get; set; } // PDF, PPT, VIDEO

        public string FilePath { get; set; }

        public DateTime UploadedAt { get; set; }

        public Module Module { get; set; }
    }
}