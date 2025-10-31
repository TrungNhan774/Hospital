using BLL.DTOs;
using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllAsync(bool showDeleted = false);
        Task<Patient> GetByIdAsync(int id);
        Task AddAsync(Patient patient);
        Task UpdateAsync(Patient patient);
        Task SoftDeleteAsync(int id);
        Task RestoreAsync(int id);
        Task HardDeleteAsync(int id);
        Task<PatientIdDto?> GetPatientIdByUserIdAsync(int userId);
    }
}