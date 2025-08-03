using System.ComponentModel.DataAnnotations;

namespace GYM_APP.ViewModels.MemberVMs
{
    public class PaymentRequestViewModel
    {
        [Required]
        public int PackageId { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        public string CardNumber { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }

        [StringLength(4, MinimumLength = 3)]
        public string Cvc { get; set; }
    }
}
