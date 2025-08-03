using System.ComponentModel.DataAnnotations;

namespace GYM_APP.ViewModels.AdminVMs
{
    public class ClassScheduleViewModel
    {
        public int ClassScheduleId { get; set; }

        [Required(ErrorMessage = "Day of week is required")]
        [RegularExpression("^(Saturday|Sunday|Monday|Tuesday|Wednesday|Thursday|Friday)$",
            ErrorMessage = "Please select a valid day of the week")]
        public string ClassScheduleDayOfWeek { get; set; }

        [Required(ErrorMessage = "Schedule time is required")]
        public TimeOnly? ClassScheduleTime { get; set; }

        [Required(ErrorMessage = "Please select a class")]
        public int? ClassesId { get; set; }
    }
}
