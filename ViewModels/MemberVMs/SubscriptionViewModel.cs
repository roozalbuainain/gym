namespace GYM_APP.ViewModels.MemberVMs
{
    public class SubscriptionViewModel
    {
        public int SubscriptionId { get; set; }
        public string PackageName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
        public DateTime? FreezeStartDate { get; set; }
        public DateTime? FreezeEndDate { get; set; }
        public int DurationDays { get; set; }
        public string Privileges { get; set; }
    }
}
