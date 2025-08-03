using System.ComponentModel.DataAnnotations;

namespace GYM_APP.ViewModels.MemberVMs
{
    public class VerifyOtpViewModel
    {
        [Required]
        [Range(100000, 999999)]
        public int Otp { get; set; }

        [Required]
        public int PackageId { get; set; }
    }
}
