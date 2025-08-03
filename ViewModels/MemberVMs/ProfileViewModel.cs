using GYM_APP.Validations;
using System.ComponentModel.DataAnnotations;

namespace GYM_APP.ViewModels.MemberVMs
{
    public class ProfileViewModel
    {
        public int UsersId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [PhoneNumber]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Range(1, 120)]
        public int? Age { get; set; }

        [Range(50, 250)]
        [Display(Name = "Height (CM)")]
        public decimal? HeightCm { get; set; }

        [Range(30, 300)]
        [Display(Name = "Weight (KG)")]
        public decimal? WeightKg { get; set; }

        [StringLength(100)]
        [Display(Name = "Body Size")]
        public string BodySize { get; set; }

        [StringLength(255)]
        [Display(Name = "Health Status")]
        public string HealthStatus { get; set; }

        [StringLength(255)]
        [Display(Name = "Fitness Goals")]
        public string FitnessGoals { get; set; }

        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
