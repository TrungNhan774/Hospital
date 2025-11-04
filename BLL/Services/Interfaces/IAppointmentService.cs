using DAL.Models;
using System;
using System.Collections.Generic;
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
            int scheduleId, int doctorId, int departmentId, int patientId, int serviceId, string? notes, int? roomId);

        Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(int patientId, string? status = null);
        Task<bool> CancelAppointmentAsync(int appointmentId);
        Task<(bool Success, string Message)> CheckScheduleAvailabilityAsync(int scheduleId, int serviceId);
    }
}
