using System;
using System.Collections.Generic;

namespace GYM_APP.Models;

public partial class Attendance
{
    public int AttendancesId { get; set; }

    public DateTime? AttendancesTime { get; set; }

    public DateTime? AttendancesCheckInTime { get; set; }

    public DateTime? AttendancesCheckOutTime { get; set; }

    public string? AttendancesStatus { get; set; }

    public string? AttendancesType { get; set; }

    public int? UsersId { get; set; }

    public virtual ICollection<MemberAttendance> MemberAttendances { get; set; } = new List<MemberAttendance>();

    public virtual User? Users { get; set; }
}
