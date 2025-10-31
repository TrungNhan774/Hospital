using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Implements;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implements
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repository;
        private readonly DbhospitalContext _context;

        public AppointmentService(IAppointmentRepository repository, DbhospitalContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync(string searchString)
        {
            return await _repository.GetAllAsync(searchString);
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(int doctorId)
        {
            return await _repository.GetAppointmentsByDoctorIdAsync(doctorId);
        }
        public async Task UpdateAppointmentStatusAsync(int appointmentId, string newStatus)
        {
            var appt = await _repository.GetByIdAsync(appointmentId);
            if (appt != null)
            {
                appt.Status = newStatus;
                await _repository.UpdateAsync(appt);
            }
        }

        public async Task<(bool Success, string Message)> BookAppointmentAsync(
           int scheduleId, int doctorId, int departmentId, int patientId, int serviceId)
        {
            var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);
            if (schedule == null)
                return (false, "Không tìm thấy lịch làm việc.");

            var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == serviceId && s.IsActive);
            if (service == null)
                return (false, "Dịch vụ không tồn tại hoặc ngưng hoạt động.");

            int serviceWeight = service.Weight;
            DateTime scheduleDate = schedule.WorkDate.ToDateTime(TimeOnly.MinValue);
            int newTotalWeight = (schedule.Weight ?? 0) + serviceWeight;
            if (newTotalWeight > 3)
                return (false, "Lịch đã đầy, không thể đặt thêm dịch vụ này.");
            var doctor = await _context.Doctors
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.DoctorId == doctorId);

            if (doctor == null)
                return (false, "Bác sĩ không tồn tại.");

            var departmentName = doctor.Department?.Name ?? "Chưa xác định";
            var appointment = new Appointment
            {
                DoctorId = doctorId,
                PatientId = patientId,
                AppointmentDate = scheduleDate,
                Status = "Pending"
            };

            await _repository.AddAsync(appointment);
            await _repository.SaveChangesAsync();
            var appointmentService = new AppointmentServiceModel
            {
                AppointmentId = appointment.AppointmentId,
                ServiceId = service.ServiceId
            };
            _context.AppointmentServiceModels.Add(appointmentService);
            schedule.Weight = newTotalWeight;
            _context.Schedules.Update(schedule);

            await _context.SaveChangesAsync();
            string message = $"Đặt lịch thành công!\n" +
                             $"Bác sĩ: {doctor.FullName}\n" +
                             $"Khoa: {departmentName}\n" +
                             $"Ngày: {schedule.WorkDate:dd/MM/yyyy}\n" +
                             $"Ca: {schedule.Shift}\n" +
                             $"Dịch vụ: {service.Name}";

            return (true, message);
        }
    }
}
