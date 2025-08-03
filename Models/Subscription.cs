using System;
using System.Collections.Generic;

namespace GYM_APP.Models;

public partial class Subscription
{
    public int SubscriptionsId { get; set; }

    public DateOnly? SubscriptionsStartDate { get; set; }

    public DateOnly? SubscriptionsEndDate { get; set; }

    public string? SubscriptionsStatus { get; set; }

    public int? PackagesId { get; set; }

    public int? UsersId { get; set; }

    public DateOnly? SubscriptionsFreezeStartDate { get; set; }

    public DateOnly? SubscriptionsFreezeEndDate { get; set; }

    public virtual Package? Packages { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User? Users { get; set; }
}
