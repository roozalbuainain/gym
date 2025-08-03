using System;
using System.Collections.Generic;

namespace GYM_APP.Models;

public partial class User
{
    public int UsersId { get; set; }

    public string? UsersName { get; set; }

    public string? UsersEmail { get; set; }

    public string? UsersPassword { get; set; }

    public string? UsersRole { get; set; }

    public DateTime? UsersJoinedAt { get; set; }

    public string? UsersPhoneNumber { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Member> Members { get; set; } = new List<Member>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
