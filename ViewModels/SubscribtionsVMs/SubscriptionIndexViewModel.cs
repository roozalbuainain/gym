using System.ComponentModel.DataAnnotations;

namespace GYM_APP.ViewModels.SubscribtionsVMs
{
    public class SubscriptionIndexViewModel
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

        [Display(Name = "User")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string UserEmail { get; set; } = string.Empty;

        [Display(Name = "Package")]
        public string PackageName { get; set; } = string.Empty;

        [Display(Name = "Package Price")]
        public decimal PackagePrice { get; set; }

        [Display(Name = "Total Payments")]
        public decimal TotalPayments { get; set; }

        [Display(Name = "Payment Count")]
        public int PaymentCount { get; set; }

        public int UsersId { get; set; }
        public int PackagesId { get; set; }
    }
}
