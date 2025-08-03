using System;
using System.Collections.Generic;

namespace GYM_APP.Models;

public partial class Class
{
    public int ClassesId { get; set; }

    public string? ClassesName { get; set; }

    public string? ClassesDescription { get; set; }

    public int? ClassesDurationMinutes { get; set; }

    public int? ClassesCapacity { get; set; }

    public int? TrainerId { get; set; }

    public virtual ICollection<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual User? Trainer { get; set; }
}
