using System;
using System.Collections.Generic;

namespace GYM_APP.Models;

public partial class Review
{
    public int ReviewsId { get; set; }

    public int? ReviewsRating { get; set; }

    public string? ReviewsComment { get; set; }

    public DateOnly? ReviewsDate { get; set; }

    public TimeOnly? ReviewsTime { get; set; }

    public int? ClassId { get; set; }

    public int? UsersId { get; set; }

    public virtual Class? Class { get; set; }

    public virtual User? Users { get; set; }
}
