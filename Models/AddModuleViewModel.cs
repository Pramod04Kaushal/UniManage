using Microsoft.AspNetCore.Http;

namespace UniManage.Models
{
    public class AddModuleViewModel
    {
        public string ModuleName { get; set; }

        public string ModuleCode { get; set; }

        public string Department { get; set; }

        public int Credits { get; set; }

        public IFormFile PdfFile { get; set; }

        public IFormFile PresentationFile { get; set; }

        public IFormFile VideoFile { get; set; }

        public string Description { get; set; }

        public int LecturerID { get; set; }

        public string Status { get; set; }
    }
}