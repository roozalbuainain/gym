using System.ComponentModel.DataAnnotations;

namespace GYM_APP.ViewModels.SubscribtionsVMs
{
    public class PaymentViewModel
    {
        public int PaymentsId { get; set; }

        [Display(Name = "Amount")]
        public decimal PaymentsAmount { get; set; }

        [Display(Name = "Payment Date")]
        public DateOnly PaymentsDate { get; set; }

        [Display(Name = "Payment Time")]
        public TimeOnly PaymentsTime { get; set; }

        [Display(Name = "Payment Method")]
        public string PaymentsMethods { get; set; } = string.Empty;

        [Display(Name = "Status")]
        public string PaymentsStatus { get; set; } = string.Empty;

        [Display(Name = "Transaction ID")]
        public string PaymentsTransactionId { get; set; } = string.Empty;
    }
}
