namespace GYM_APP.ViewModels.TrainerVMs
{
    public class TrainerHomeViewModel
    {
        public List<TrainerClassViewModel> Classes { get; set; } = new();
        public List<TrainerScheduleViewModel> Schedules { get; set; } = new();
    }
}
