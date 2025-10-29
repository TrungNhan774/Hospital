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

        public AppointmentService(IAppointmentRepository repository)
        {
            _repository = repository;
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
    }
}
