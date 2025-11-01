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
    int scheduleId, int doctorId, int departmentId, int patientId, int serviceId, string? notes, int? roomId)
        {
            var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);
            if (schedule == null)
                return (false, "Schedule not found.");

            var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == serviceId && s.IsActive);
            if (service == null)
                return (false, "Service not found or inactive.");

            int serviceWeight = service.Weight;
            DateTime scheduleDate = schedule.WorkDate.ToDateTime(TimeOnly.MinValue);

            int newTotalWeight = (schedule.Weight ?? 0) + serviceWeight;
            if (newTotalWeight > 3)
                return (false, "The schedule is full, cannot add this service.");

            var doctor = await _context.Doctors
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.DoctorId == doctorId);
            if (doctor == null)
                return (false, "Doctor not found.");

            var departmentName = doctor.Department?.Name ?? "Unknown";

            // ✅ Nếu có roomId thì chỉ cần kiểm tra tồn tại phòng thôi
            Room? room = null;
            if (roomId.HasValue)
            {
                room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == roomId);
                if (room == null)
                    return (false, "Room not found.");
            }

            // ✅ Tạo appointment
            var appointment = new Appointment
            {
                DoctorId = doctorId,
                PatientId = patientId,
                AppointmentDate = scheduleDate,
                Status = "Pending",
                Notes = notes,
                RoomId = roomId // Gán phòng (nếu có)
            };

            await _repository.AddAsync(appointment);
            await _repository.SaveChangesAsync();

            // ✅ Thêm service cho appointment
            var appointmentService = new AppointmentServiceModel
            {
                AppointmentId = appointment.AppointmentId,
                ServiceId = service.ServiceId
            };
            _context.AppointmentServiceModels.Add(appointmentService);

            // ✅ Cập nhật trọng số lịch
            schedule.Weight = newTotalWeight;
            _context.Schedules.Update(schedule);

            await _context.SaveChangesAsync();

            string message = $"✅ Appointment booked successfully!\n" +
                             $"Doctor: {doctor.FullName}\n" +
                             $"Department: {departmentName}\n" +
                             $"Date: {schedule.WorkDate:dd/MM/yyyy}\n" +
                             $"Shift: {schedule.Shift}\n" +
                             $"Service: {service.Name}\n" +
                             $"{(room != null ? $"Room: {room.RoomNumber}" : "No room assigned")}";

            return (true, message);
        }

    }
}
