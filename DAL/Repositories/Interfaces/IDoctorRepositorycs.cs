using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<Doctor>> GetAllAsync(string searchString = null);
        Task<Doctor> GetByIdAsync(int id);
        Task AddAsync(Doctor doctor);
        Task UpdateAsync(Doctor doctor);
        Task DeleteAsync(int id);
        Task<Doctor?> GetByUserIdAsync(int userId);
        Task<IEnumerable<Doctor>> GetByDepartmentAsync(int? departmentId, string searchString = null);
        bool Exists(int id);
        Task<Doctor?> GetDoctorProfileByUserIdAsync(int userId);
        Task<Doctor?> GetDoctorWithDetailsAsync(int doctorId);
        Task<IEnumerable<Doctor>> GetByDepartmentIdAsync(int departmentId);

    }
}
