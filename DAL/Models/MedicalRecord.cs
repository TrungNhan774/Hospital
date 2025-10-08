using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models;

public partial class MedicalRecord
{
    [Key]
    public int RecordId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public string? Diagnosis { get; set; }

    public string? Prescription { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual ICollection<MedicalRecordMedicine> MedicalRecordMedicines { get; set; } = new List<MedicalRecordMedicine>();

    public virtual Patient Patient { get; set; } = null!;
}
