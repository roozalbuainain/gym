namespace GYM_APP.ViewModels.MemberVMs
{
    public class ClassScheduleViewModel
    {
        public int ScheduleId { get; set; }
        public string DayOfWeek { get; set; }
        public TimeOnly? Time { get; set; }
        public int BookedCount { get; set; }
        public bool IsBooked { get; set; }
    }
}
