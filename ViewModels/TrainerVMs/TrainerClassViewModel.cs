namespace GYM_APP.ViewModels.TrainerVMs
{
    public class TrainerClassViewModel
    {
        public int ClassesId { get; set; }
        public string ClassesName { get; set; } = string.Empty;
        public string? ClassesDescription { get; set; }
        public int ClassesCapacity { get; set; }
        public int ClassesDurationMinutes { get; set; }
        public int SchedulesCount { get; set; }

        public int BookingsCount { get; set; }
    }
}
