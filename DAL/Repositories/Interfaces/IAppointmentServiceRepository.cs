using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IAppointmentServiceRepository
    {
        Task<IEnumerable<AppointmentServiceModel>> GetAllAsync();
        Task<AppointmentServiceModel?> GetByIdsAsync(int appointmentId, int serviceId);
        Task<IEnumerable<AppointmentServiceModel>> GetByAppointmentIdAsync(int appointmentId);
        Task AddAsync(AppointmentServiceModel entity);
        Task DeleteAsync(int appointmentId, int serviceId);
        Task SaveChangesAsync();
    }
}
