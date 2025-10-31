namespace BLL.DTOs
{
    public class PatientMedicalRecordDto
    {
        public int RecordId { get; set; }
        public string DoctorName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Diagnosis { get; set; }
        public string? Prescription { get; set; }
    }
}