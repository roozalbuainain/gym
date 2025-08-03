using System.ComponentModel.DataAnnotations;

namespace GYM_APP.ViewModels.PackageVMs
{
    public class PackageIndexViewModel
    {
        public int PackagesId { get; set; }

        [Display(Name = "Package Name")]
        public string PackagesName { get; set; } = string.Empty;

        [Display(Name = "Duration (days)")]
        public int PackagesDurationDays { get; set; }

        [Display(Name = "Price")]
        public decimal PackagesPrice { get; set; }

        [Display(Name = "Type")]
        public string PackagesType { get; set; } = string.Empty;

        [Display(Name = "Max Freeze Days")]
        public int PackagesMaxFreezeDays { get; set; }

        [Display(Name = "Refund Policy Days")]
        public int RefundPolicyDays { get; set; }

        [Display(Name = "Daily Charge")]
        public decimal DailyChargeIfStarted { get; set; }

        [Display(Name = "Privileges")]
        public string? Privileges { get; set; }
    }
}
