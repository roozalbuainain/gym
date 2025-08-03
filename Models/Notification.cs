using System;
using System.Collections.Generic;

namespace GYM_APP.Models;

public partial class Notification
{
    public int NotificationsId { get; set; }

    public string? NotificationsMessage { get; set; }

    public string? NotificationsType { get; set; }

    public bool? NotificationsIsRead { get; set; }

    public int? UsersId { get; set; }

    public virtual User? Users { get; set; }
}
