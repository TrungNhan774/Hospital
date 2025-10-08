using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models;

public partial class Room
{
    [Key]
    public int RoomId { get; set; }

    public int DepartmentId { get; set; }

    public string RoomNumber { get; set; } = null!;

    public string? Type { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Department Department { get; set; } = null!;
}
