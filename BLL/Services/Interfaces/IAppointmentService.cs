using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAllAsync(string searchString);
        Task<Appointment?> GetByIdAsync(int id);
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(int doctorId);
        Task UpdateAppointmentStatusAsync(int appointmentId, string newStatus);

        Task<(bool Success, string Message)> BookAppointmentAsync(
            int scheduleId, int doctorId, int departmentId, int patientId, int serviceWeight);
    }
}
