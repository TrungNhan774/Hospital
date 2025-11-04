using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services.Implements
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IScheduleRepository _scheduleRepo;
        private readonly IServiceRepository _serviceRepo;
        private readonly IDoctorRepository _doctorRepo;
        private readonly IRoomRepository _roomRepo;
        private readonly IAppointmentServiceRepository _appointmentServiceRepo;

        public AppointmentService(
            IAppointmentRepository appointmentRepo,
            IScheduleRepository scheduleRepo,
            IServiceRepository serviceRepo,
            IDoctorRepository doctorRepo,
            IRoomRepository roomRepo,
            IAppointmentServiceRepository appointmentServiceRepo)
        {
            _appointmentRepo = appointmentRepo;
            _scheduleRepo = scheduleRepo;
            _serviceRepo = serviceRepo;
            _doctorRepo = doctorRepo;
            _roomRepo = roomRepo;
            _appointmentServiceRepo = appointmentServiceRepo;
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync(string searchString)
        {
            return await _appointmentRepo.GetAllAsync(searchString);
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _appointmentRepo.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(int doctorId)
        {
            return await _appointmentRepo.GetAppointmentsByDoctorIdAsync(doctorId);
        }

        public async Task UpdateAppointmentStatusAsync(int appointmentId, string newStatus)
        {
            var appt = await _appointmentRepo.GetByIdAsync(appointmentId);
            if (appt != null)
            {
                appt.Status = newStatus;
                await _appointmentRepo.UpdateAsync(appt);
            }
        }

        public async Task<(bool Success, string Message)> BookAppointmentAsync(
            int scheduleId, int doctorId, int departmentId, int patientId, int serviceId, string? notes, int? roomId)
        {
            var schedule = await _scheduleRepo.GetByIdAsync(scheduleId);
            if (schedule == null)
                return (false, "Schedule not found.");

            var service = await _serviceRepo.GetByIdAsync(serviceId);
            if (service == null || !service.IsActive)
                return (false, "Service not found or inactive.");

            int newWeight = (schedule.Weight ?? 0) + service.Weight;
            if (newWeight > 3)
                return (false, "The schedule is full, cannot add this service.");

            var doctor = await _doctorRepo.GetByIdAsync(doctorId);
            if (doctor == null)
                return (false, "Doctor not found.");

            var departmentName = doctor.Department?.Name ?? "Unknown";

            Room? room = null;
            if (roomId.HasValue)
            {
                room = await _roomRepo.GetByIdAsync(roomId.Value);
                if (room == null)
                    return (false, "Room not found.");
            }

            // ✅ Tạo appointment
            var appointment = new Appointment
            {
                DoctorId = doctorId,
                PatientId = patientId,
                AppointmentDate = schedule.WorkDate.ToDateTime(TimeOnly.MinValue),
                Status = "PENDING",
                Notes = notes,
                RoomId = roomId
            };

            await _appointmentRepo.AddAsync(appointment);
            await _appointmentRepo.SaveChangesAsync();

            // ✅ Tạo record cho bảng Appointment_Service
            var apService = new AppointmentServiceModel
            {
                AppointmentId = appointment.AppointmentId,
                ServiceId = serviceId
            };

            await _appointmentServiceRepo.AddAsync(apService);
            await _appointmentServiceRepo.SaveChangesAsync();

            // ✅ Cập nhật lại trọng số lịch
            schedule.Weight = newWeight;
            await _scheduleRepo.UpdateAsync(schedule);

            string message = $"✅ Appointment booked successfully!\n" +
                             $"Doctor: {doctor.FullName}\n" +
                             $"Department: {departmentName}\n" +
                             $"Date: {schedule.WorkDate:dd/MM/yyyy}\n" +
                             $"Shift: {schedule.Shift}\n" +
                             $"Service: {service.Name}\n" +
                             $"{(room != null ? $"Room: {room.RoomNumber}" : "No room assigned")}";

            return (true, message);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(int patientId, string? status = null)
        {
            var list = await _appointmentRepo.GetAppointmentsByPatientAsync(patientId);
            if (!string.IsNullOrEmpty(status))
                list = list.Where(a => a.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            return list.OrderByDescending(a => a.AppointmentDate);
        }

        public async Task<bool> CancelAppointmentAsync(int appointmentId)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(appointmentId);
            if (appointment == null)
                return false;

            appointment.Status = "CANCELLED";
            await _appointmentRepo.UpdateAsync(appointment);
            return true;
        }

        public async Task<(bool Success, string Message)> CheckScheduleAvailabilityAsync(int scheduleId, int serviceId)
        {
            var schedule = await _scheduleRepo.GetByIdAsync(scheduleId);
            if (schedule == null)
                return (false, "Schedule not found.");

            var service = await _serviceRepo.GetByIdAsync(serviceId);
            if (service == null || !service.IsActive)
                return (false, "Service not found or inactive.");

            int newWeight = (schedule.Weight ?? 0) + service.Weight;
            if (newWeight > 3)
                return (false, "The schedule is full, please choose another slot.");

            return (true, "Available");
        }
    }
}
