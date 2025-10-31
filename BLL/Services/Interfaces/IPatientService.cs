using DAL.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public interface IPatientService
    {
        Task<IEnumerable<Patient>> GetAllAsync(bool showDeleted = false);
        Task<Patient> GetByIdAsync(int id);
        Task AddAsync(Patient patient);
        Task UpdateAsync(Patient patient);
        Task DeleteAsync(int id);
        IEnumerable<User> GetAllUsers();
    }
}