namespace DAL.Models.DTO
{
    public class DoctorDTO
    {
        public int DoctorId { get; set; }

        public string FullName { get; set; } = null!;

        public string Qualification { get; set; } = null!;

        public int ExperienceYears { get; set; }

        public string? About { get; set; }
    }
}
