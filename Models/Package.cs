using System;
using System.Collections.Generic;

namespace GYM_APP.Models;

public partial class Package
{
    public int PackagesId { get; set; }

    public string? PackagesName { get; set; }

    public int? PackagesDurationDays { get; set; }

    public decimal? PackagesPrice { get; set; }

    public string? PackagesType { get; set; }

    public int? PackagesMaxFreezeDays { get; set; }

    public int? RefundPolicyDays { get; set; }

    public decimal? DailyChargeIfStarted { get; set; }

    public string? Privileges { get; set; }

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
