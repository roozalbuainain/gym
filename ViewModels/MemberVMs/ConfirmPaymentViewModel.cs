namespace GYM_APP.ViewModels.MemberVMs
{
    public class ConfirmPaymentViewModel
    {
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public decimal PackagePrice { get; set; }
        public string UserName { get; set; }
        public string CardLastFour { get; set; }
        public DateTime PaymentTime { get; set; }
        public int Otp { get; set; }
    }
}
