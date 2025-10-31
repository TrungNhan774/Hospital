// DAL/Models/DTO/DepartmentDTO.cs
using System.ComponentModel.DataAnnotations;

namespace DAL.Models.DTO
{
    public class DepartmentDTO
    {
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Department name is required.")]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        // Thêm thuộc tính để hiển thị số lượng bác sĩ, phòng
        public int DoctorCount { get; set; }
        public int RoomCount { get; set; }

        // ⭐ THÊM THUỘC TÍNH NÀY cho Soft Delete
        public bool IsDeleted { get; set; }
    }
}