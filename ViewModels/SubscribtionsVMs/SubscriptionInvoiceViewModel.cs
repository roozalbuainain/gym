using System.ComponentModel.DataAnnotations;

namespace GYM_APP.ViewModels.SubscribtionsVMs
{
    public class SubscriptionInvoiceViewModel
    {
        public int SubscriptionsId { get; set; }

        [Display(Name = "Start Date")]
        public DateOnly SubscriptionsStartDate { get; set; }

        [Display(Name = "End Date")]
        public DateOnly SubscriptionsEndDate { get; set; }

        [Display(Name = "Status")]
        public string SubscriptionsStatus { get; set; } = string.Empty;

        [Display(Name = "Freeze Start Date")]
        public DateOnly? SubscriptionsFreezeStartDate { get; set; }

        [Display(Name = "Freeze End Date")]
        public DateOnly? SubscriptionsFreezeEndDate { get; set; }

        // User Details
        [Display(Name = "User Name")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string UserEmail { get; set; } = string.Empty;

        [Display(Name = "Phone")]
        public string UserPhone { get; set; } = string.Empty;

        [Display(Name = "Joined Date")]
        public DateTime UserJoinedAt { get; set; }

        // Package Details
        [Display(Name = "Package Name")]
        public string PackageName { get; set; } = string.Empty;

        [Display(Name = "Package Price")]
        public decimal PackagePrice { get; set; }

        [Display(Name = "Package Type")]
        public string PackageType { get; set; } = string.Empty;

        [Display(Name = "Duration (days)")]
        public int PackageDurationDays { get; set; }

        [Display(Name = "Max Freeze Days")]
        public int PackageMaxFreezeDays { get; set; }

        [Display(Name = "Refund Policy Days")]
        public int RefundPolicyDays { get; set; }

        [Display(Name = "Daily Charge")]
        public decimal DailyChargeIfStarted { get; set; }

        [Display(Name = "Privileges")]
        public string? Privileges { get; set; }

        // Payment Details
        public List<PaymentViewModel> Payments { get; set; } = new();

        [Display(Name = "Total Amount Paid")]
        public decimal TotalAmountPaid { get; set; }
    }
}
