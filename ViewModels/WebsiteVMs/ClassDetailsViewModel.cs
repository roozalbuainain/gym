namespace GYM_APP.ViewModels.WebsiteVMs
{
    public class ClassDetailsViewModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        public int Capacity { get; set; }
        public List<ClassScheduleViewModel> Schedules { get; set; } = new List<ClassScheduleViewModel>();
        public List<ClassReviewViewModel> Reviews { get; set; } = new List<ClassReviewViewModel>();
        public double? AvgRating { get; set; }
        public bool IsSubscribed { get; set; }
        public string TrainerName { get; set; }
    }
}
