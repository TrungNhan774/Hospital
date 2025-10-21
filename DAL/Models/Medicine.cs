using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models;

public partial class Medicine
{
    [Key]
    public int MedicineId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Unit { get; set; }

    public decimal Price { get; set; }

    public DateTime? CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<MedicalRecordMedicine> MedicalRecordMedicines { get; set; } = new List<MedicalRecordMedicine>();
}
