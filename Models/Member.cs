using System;
using System.Collections.Generic;

namespace GYM_APP.Models;

public partial class Member
{
    public int MemberId { get; set; }

    public int? MemberAge { get; set; }

    public int? MembeHeightCm { get; set; }

    public int? MemberWeightKg { get; set; }

    public string? MemberBodySize { get; set; }

    public string? MemberHealthStatus { get; set; }

    public string? MemberFitnessGoals { get; set; }

    public int? UsersId { get; set; }

    public virtual User? Users { get; set; }
}
