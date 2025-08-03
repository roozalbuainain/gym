using GYM_APP.ViewModels.WebsiteVMs;

namespace GYM_APP.ViewModels.MemberVMs
{
    public class MyClassesViewModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        public int Capacity { get; set; }
        public List<ClassScheduleViewModel> Schedules { get; set; } = new List<ClassScheduleViewModel>();
    }
}
