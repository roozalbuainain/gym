namespace GYM_APP.ViewModels.WebsiteVMs
{
    public class ClassViewModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        public int? DurationMinutes { get; set; }
        public int? Capacity { get; set; }
        public string TrainerName { get; set; }
        public List<ClassScheduleViewModel> Schedules { get; set; } = new List<ClassScheduleViewModel>();

        // New properties for ratings and reviews
        public double? AvgRating { get; set; }
        public List<ClassReviewViewModel> Reviews { get; set; } = new List<ClassReviewViewModel>();
        public bool IsSubscribed { get; set; }
    }
}
