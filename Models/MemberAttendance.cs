using System;
using System.Collections.Generic;

namespace GYM_APP.Models;

public partial class MemberAttendance
{
    public int MemberAttendanceId { get; set; }

    public int ClassScheduleId { get; set; }

    public int BookingId { get; set; }

    public int AttendancesId { get; set; }

    public virtual Attendance Attendances { get; set; } = null!;

    public virtual Booking Booking { get; set; } = null!;

    public virtual ClassSchedule ClassSchedule { get; set; } = null!;
}
