using System.ComponentModel.DataAnnotations;

namespace GYM_APP.ViewModels.PackageVMs
{
    public class PackageCreateViewModel
    {
        [Required(ErrorMessage = "Package Name is required")]
        [StringLength(255, ErrorMessage = "Package Name cannot exceed 255 characters")]
        [Display(Name = "Package Name")]
        public string PackagesName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Duration (days) is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 day")]
        [Display(Name = "Duration (days)")]
        public int PackagesDurationDays { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be 0 or greater")]
        [Display(Name = "Price")]
        public decimal PackagesPrice { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [Display(Name = "Type")]
        public string PackagesType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Max Freeze Days is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Max Freeze Days must be 0 or greater")]
        [Display(Name = "Max Freeze Days")]
        public int PackagesMaxFreezeDays { get; set; }

        [Required(ErrorMessage = "Refund Policy Days is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Refund Policy Days must be 0 or greater")]
        [Display(Name = "Refund Policy Days")]
        public int RefundPolicyDays { get; set; }

        [Required(ErrorMessage = "Daily Charge is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Daily Charge must be 0 or greater")]
        [Display(Name = "Daily Charge")]
        public decimal DailyChargeIfStarted { get; set; }

        [Display(Name = "Privileges")]
        public string? Privileges { get; set; }
    }

}
