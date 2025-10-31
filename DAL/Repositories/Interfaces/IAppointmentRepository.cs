using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetAppointmentsByDoctorAndDateAsync(int doctorId, DateTime date);
        Task AddAsync(Appointment appointment);
        Task SaveChangesAsync();
    }
}
