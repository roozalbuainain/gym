using System.ComponentModel.DataAnnotations;

namespace GYM_APP.ViewModels.FeedbackVMs
{
    public class ClassViewModel
    {
        public int ClassesId { get; set; }

        [Display(Name = "Class Name")]
        public string ClassesName { get; set; } = string.Empty;
    }
}
