using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public partial class Patient
{
    [Key]
    public int PatientId { get; set; }

    [Required(ErrorMessage = "Please select a user")]
    public int UserId { get; set; }
    [Required(ErrorMessage = "Please enter patient name")]
    [Column("patient_name")]
    public string PatientName { get; set; }

    [StringLength(10)]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Please enter date of birth")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }  // đổi từ DateOnly? sang DateTime?

    [Required(ErrorMessage = "Please enter gender")]
    public string? Gender { get; set; }

    [Required(ErrorMessage = "Please enter address")]
    public string? Address { get; set; }

    public string? MedicalHistory { get; set; }
    public bool IsDeleted { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User User { get; set; } = null!;
}