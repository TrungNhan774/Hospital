using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models.DTO
{
    public class PatientDTO
    {
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Please select a user")]
        public int UserId { get; set; }

        public string? UserFullName { get; set; }  // ✅ Thêm dòng này để hiển thị tên user

        [Required(ErrorMessage = "Please enter date of birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Please enter gender")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Please enter address")]
        public string? Address { get; set; }

        public string? MedicalHistory { get; set; }
    }
}
