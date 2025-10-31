namespace BLL.DTOs
{
    public class PatientMedicalRecordDetailDto
    {
        public int RecordId { get; set; }
        public string DoctorName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Diagnosis { get; set; }
        public string? Prescription { get; set; }
        public List<MedicineDto> PrescribedMedicines { get; set; } = new List<MedicineDto>();
    }
}