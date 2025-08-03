using System;
using System.Collections.Generic;

namespace GYM_APP.Models;

public partial class ClassSchedule
{
    public int ClassScheduleId { get; set; }

    public string? ClassScheduleDayOfWeek { get; set; }

    public TimeOnly? ClassScheduleTime { get; set; }

    public int? ClassesId { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Class? Classes { get; set; }

    public virtual ICollection<MemberAttendance> MemberAttendances { get; set; } = new List<MemberAttendance>();
}
