using System;
using System.Collections.Generic;

namespace GYM_APP.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public decimal InvTotalAmount { get; set; }

    public int UsersId { get; set; }

    public int PaymentsId { get; set; }

    public virtual Payment Payments { get; set; } = null!;

    public virtual User Users { get; set; } = null!;
}
