// Trong DAL\Models\AppointmentService.cs

namespace DAL.Models
{
    public class AppointmentServiceModel
    {
        // Khóa ngoại 1: Liên kết tới Appointment
        public int AppointmentId { get; set; }
        public virtual Appointment Appointment { get; set; }

        // Khóa ngoại 2: Liên kết tới Service
        public int ServiceId { get; set; }
        public virtual Service Service { get; set; }

        // (Bạn không cần trường Weight ở đây vì nó nằm trong Service)
    }
}