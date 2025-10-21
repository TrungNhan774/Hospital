using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models;

public partial class Room
{
    [Key]
    public int RoomId { get; set; }

    [Required(ErrorMessage = "Please select a department.")]
    public int? DepartmentId { get; set; }
    [Required(ErrorMessage = "Room number is required.")]
    public string RoomNumber { get; set; } = null!;

    [Required(ErrorMessage = "Please select a room type.")]
    public string? Type { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Department? Department { get; set; } = null!;
}
