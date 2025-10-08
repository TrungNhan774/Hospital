using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class MedicalRecordMedicine
{
    public int RecordId { get; set; }

    public int MedicineId { get; set; }

    public string? Dosage { get; set; }

    public int? Quantity { get; set; }

    public virtual Medicine Medicine { get; set; } = null!;

    public virtual MedicalRecord Record { get; set; } = null!;
}
