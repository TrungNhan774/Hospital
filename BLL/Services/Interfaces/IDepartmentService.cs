// BLL/Services/IDepartmentService.cs (Giao diện mới)
using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetAllAsync(bool showDeleted = false);
        Task<Department> GetByIdAsync(int id);
        Task AddAsync(Department department);
        Task UpdateAsync(Department department);
        Task DeleteAsync(int id); 
        Task RestoreAsync(int id);
        Task<bool> ExistsWithNameAsync(string name, int? excludeId = null);
    }
}