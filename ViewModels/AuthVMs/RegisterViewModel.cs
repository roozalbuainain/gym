using System.ComponentModel.DataAnnotations;

namespace GYM_APP.ViewModels.AuthVMs
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [RegularExpression(@"^(\+?20|0)?1[0125]\d{8}$", ErrorMessage = "Please enter a valid Egyptian phone number")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "National ID must be exactly 10 digits")]
        [Display(Name = "National ID")]
        public string NationalId { get; set; }

        [Range(10, 100, ErrorMessage = "Age must be between 10 and 100")]
        [Display(Name = "Age")]
        public int? Age { get; set; }

        [Range(20, 300, ErrorMessage = "Weight must be between 20 and 300 kg")]
        [Display(Name = "Weight (kg)")]
        public double? Weight { get; set; }

        [Range(50, 300, ErrorMessage = "Height must be between 50 and 300 cm")]
        [Display(Name = "Height (cm)")]
        public double? Height { get; set; }

        [StringLength(255)]
        [Display(Name = "Health Issues")]
        public string? HealthIssues { get; set; }

        [Required(ErrorMessage = "You must accept the terms and conditions")]
        [Display(Name = "I accept the terms and conditions")]
        public bool Terms { get; set; }
    }
}
