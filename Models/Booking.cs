using System;
using System.Collections.Generic;

namespace GYM_APP.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? UsersId { get; set; }

    public int? ClassScheduleId { get; set; }

    public virtual ClassSchedule? ClassSchedule { get; set; }

    public virtual ICollection<MemberAttendance> MemberAttendances { get; set; } = new List<MemberAttendance>();

    public virtual User? Users { get; set; }
}
