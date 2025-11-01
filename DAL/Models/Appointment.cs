using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models;

public partial class Appointment
{
    [Key]
    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public int? RoomId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Room? Room { get; set; }

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    public virtual ICollection<AppointmentServiceModel> AppointmentServices { get; set; } = new List<AppointmentServiceModel>();
}
