using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<IEnumerable<Doctor>> GetAllDoctorsAsync(string searchString = null);
        Task<Doctor> GetDoctorByIdAsync(int id);
        Task CreateDoctorAsync(Doctor doctor);
        Task UpdateDoctorAsync(Doctor doctor);
        Task DeleteDoctorAsync(int id);
        bool DoctorExists(int id);
        Task<Doctor?> GetDoctorProfileByUserIdAsync(int userId);
    }
}
