using Microsoft.AspNetCore.Http;
using System;

namespace UniManage.Models
{
    public class AddAssignmentViewModel
    {
        public int CourseID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int ModuleID { get; set; }

        public int BatchID { get; set; }

        public DateTime Deadline { get; set; }

        public IFormFile? AttachmentFile { get; set; }
    }
}