using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Implements
{
    public class AppointmentServiceRepository : IAppointmentServiceRepository
    {
        private readonly DbhospitalContext _context;

        public AppointmentServiceRepository(DbhospitalContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AppointmentServiceModel>> GetAllAsync()
        {
            return await _context.AppointmentServiceModels
                .Include(x => x.Service)
                .Include(x => x.Appointment)
                .ToListAsync();
        }

        public async Task<AppointmentServiceModel?> GetByIdsAsync(int appointmentId, int serviceId)
        {
            return await _context.AppointmentServiceModels
                .Include(x => x.Service)
                .Include(x => x.Appointment)
                .FirstOrDefaultAsync(x => x.AppointmentId == appointmentId && x.ServiceId == serviceId);
        }

        public async Task<IEnumerable<AppointmentServiceModel>> GetByAppointmentIdAsync(int appointmentId)
        {
            return await _context.AppointmentServiceModels
                .Include(x => x.Service)
                .Where(x => x.AppointmentId == appointmentId)
                .ToListAsync();
        }

        public async Task AddAsync(AppointmentServiceModel entity)
        {
            await _context.AppointmentServiceModels.AddAsync(entity);
        }

        public async Task DeleteAsync(int appointmentId, int serviceId)
        {
            var entity = await GetByIdsAsync(appointmentId, serviceId);
            if (entity != null)
            {
                _context.AppointmentServiceModels.Remove(entity);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
