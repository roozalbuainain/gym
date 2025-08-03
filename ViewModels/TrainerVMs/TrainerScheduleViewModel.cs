namespace GYM_APP.ViewModels.TrainerVMs
{
    public class TrainerScheduleViewModel
    {
        public int ClassScheduleId { get; set; }
        public string ClassScheduleDayOfWeek { get; set; } = string.Empty;
        public TimeOnly ClassScheduleTime { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int ClassDuration { get; set; }
        public int ClassCapacity { get; set; }

        public int BookingsCount { get; set; }
    }
}
