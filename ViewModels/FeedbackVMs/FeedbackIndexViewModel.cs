namespace GYM_APP.ViewModels.FeedbackVMs
{
    public class FeedbackIndexViewModel
    {
        public List<ReviewViewModel> Reviews { get; set; } = new();
        public List<ClassViewModel> Classes { get; set; } = new();
        public int? SelectedClassId { get; set; }
    }
}
