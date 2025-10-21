using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    [Table("Doctors")]
    public partial class Doctor
    {
        [Key]
        [Column("doctor_id")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "User is required.")]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [Column("department_id")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        [Column("full_name")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Gender is required.")]
        [StringLength(10)]
        [Column("gender")]
        public string Gender { get; set; } = null!;

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [Column("date_of_birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(20)]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [Column("phone")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Column("email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(255)]
        [Column("address")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Qualification is required.")]
        [StringLength(255)]
        [Column("qualification")]
        public string Qualification { get; set; } = null!;

        [Required(ErrorMessage = "Specialization is required.")]
        [StringLength(100)]
        [Column("specialization")]
        public string Specialization { get; set; } = null!;

        [Required(ErrorMessage = "Experience years is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Experience years must be a positive number.")]
        [Column("experience_years")]
        public int ExperienceYears { get; set; }

        [Column("about")]
        public string? About { get; set; }

        [Column("photo")]
        public string? Photo { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        // --- Navigation properties ---
        public virtual Department? Department { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}
