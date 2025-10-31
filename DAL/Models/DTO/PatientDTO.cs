using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models.DTO
{
    public class PatientDTO
    {
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Please select a user")]
        public int UserId { get; set; }

        public string? UserFullName { get; set; }

        [Required(ErrorMessage = "Please enter patient name")]
        [StringLength(100)]
        public string? PatientName { get; set; }

        [Phone]
        [StringLength(10, ErrorMessage = "Phone number must be 10 digits")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Please enter date of birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Please enter gender")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Please enter address")]
        public string? Address { get; set; }

        public string? MedicalHistory { get; set; }

        public bool IsDeleted { get; set; } // ✅ để hiển thị hoặc lọc bệnh nhân đã xóa
    }
}
