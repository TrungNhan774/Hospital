using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models;

public partial class Payment
{
    [Key]
    public int PaymentId { get; set; }

    public int PatientId { get; set; }

    public int? AppointmentId { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Appointment? Appointment { get; set; }

    public virtual Patient Patient { get; set; } = null!;
}
