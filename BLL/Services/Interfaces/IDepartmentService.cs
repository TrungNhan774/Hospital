using DAL.Models;
using System.Collections.Generic;

namespace BLL.Services
{
    public interface IDepartmentService
    {
        IEnumerable<Department> GetAll();
        Department? GetById(int id);
        void Add(Department department);
        void Update(Department department);
        void Delete(int id);
        Task<IEnumerable<Department>> GetAllAsync(bool showDeleted = false);
        Task<Department> GetByIdAsync(int id);
        Task AddAsync(Department department);
        Task UpdateAsync(Department department);
        Task DeleteAsync(int id);
        Task RestoreAsync(int id);
        Task<bool> ExistsWithNameAsync(string name, int? excludeId = null);
    }
}
