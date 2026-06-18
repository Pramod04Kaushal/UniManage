using UniManage.Models;

namespace UniManage.Models.ViewModels
{
    public class ProgramModuleViewModel
    {
        public string ModuleName { get; set; }

        public string ModuleCode { get; set; }

        public int Semester { get; set; }

        public int Credits { get; set; }
    }

    public class ProgramDetailsViewModel
    {
        public Course Course { get; set; }

        public List<ProgramModuleViewModel> Modules { get; set; }
    }
}