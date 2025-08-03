using System.ComponentModel.DataAnnotations;

namespace GYM_APP.ViewModels.FeedbackVMs
{
    public class ReviewViewModel
    {
        public int ReviewsId { get; set; }

        [Display(Name = "Rating")]
        public int ReviewsRating { get; set; }

        [Display(Name = "Comment")]
        public string ReviewsComment { get; set; } = string.Empty;

        [Display(Name = "Date")]
        public DateOnly ReviewsDate { get; set; }

        [Display(Name = "Time")]
        public TimeOnly ReviewsTime { get; set; }

        [Display(Name = "User")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Class")]
        public string ClassName { get; set; } = string.Empty;

        public int ClassId { get; set; }
        public int UsersId { get; set; }
    }
}
