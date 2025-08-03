using System;
using System.Collections.Generic;

namespace GYM_APP.Models;

public partial class Payment
{
    public int PaymentsId { get; set; }

    public decimal? PaymentsAmouent { get; set; }

    public string? PaymentsMethods { get; set; }

    public string? PaymentsTransactionId { get; set; }

    public string? PaymentsStatus { get; set; }

    public DateOnly? PaymentsDate { get; set; }

    public TimeOnly? PaymentsTime { get; set; }

    public int? UsersId { get; set; }

    public int? SubscriptionsId { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual Subscription? Subscriptions { get; set; }

    public virtual User? Users { get; set; }
}
