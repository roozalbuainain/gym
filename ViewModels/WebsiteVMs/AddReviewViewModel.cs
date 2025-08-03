using System.ComponentModel.DataAnnotations;

namespace GYM_APP.ViewModels.WebsiteVMs
{
    public class AddReviewViewModel
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        [Display(Name = "Rating")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters")]
        [Display(Name = "Comment")]
        public string? Comment { get; set; }

        [Required]
        public int ClassId { get; set; }
    }
}
