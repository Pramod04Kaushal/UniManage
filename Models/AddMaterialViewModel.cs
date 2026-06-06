using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace UniManage.Models
{
    public class AddMaterialViewModel
    {
        public int ModuleID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string MaterialType { get; set; }

        [Required]
        public IFormFile File { get; set; }
    }
}