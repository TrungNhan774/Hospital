using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models;

public partial class Schedule
{
    [Key]
    public int ScheduleId { get; set; }

    [Required(ErrorMessage = "Doctor is required.")]
    public int DoctorId { get; set; }

    [Required(ErrorMessage = "WorkDate is required.")]
    public DateOnly WorkDate { get; set; }

    [Required(ErrorMessage = "Shift is required.")]
    public string? Shift { get; set; }

    public bool? Available { get; set; }

    public virtual Doctor? Doctor { get; set; } = null!;
}
