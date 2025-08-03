using System.ComponentModel.DataAnnotations;

namespace GYM_APP.ViewModels.ClassVMs
{
    public class ClassCreateViewModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Class Name")]
        public string ClassesName { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? ClassesDescription { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute")]
        [Display(Name = "Duration (Minutes)")]
        public int ClassesDurationMinutes { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1")]
        [Display(Name = "Capacity")]
        public int ClassesCapacity { get; set; }

        [Required]
        [Display(Name = "Trainer")]
        public int TrainerId { get; set; }
    }

}
