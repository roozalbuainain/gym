namespace GYM_APP.ViewModels.MemberVMs
{
    public class PaymentViewModel
    {
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public decimal PackagePrice { get; set; }
        public string PackageType { get; set; }
        public int DurationDays { get; set; }
    }
}
