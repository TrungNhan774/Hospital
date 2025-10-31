using DAL.Models;
using DAL.Repositories;

namespace BLL.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repo;

        public DepartmentService(IDepartmentRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<Department> GetAll() => _repo.GetAll();

        public Department? GetById(int id) => _repo.GetById(id);

        public void Add(Department department) => _repo.Add(department);

        public void Update(Department department) => _repo.Update(department);

        public void Delete(int id) => _repo.Delete(id);
        public async Task<IEnumerable<Department>> GetAllAsync(bool showDeleted = false) => await _repo.GetAllAsync(showDeleted);
        public async Task<Department> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);
        public async Task AddAsync(Department department)
        {
            if (await ExistsWithNameAsync(department.Name))
            {
                // ⭐ English Error Message for Add
                throw new InvalidOperationException($"The department name '{department.Name}' already exists. Please choose a different name.");
            }
            await _repo.AddAsync(department);
        }

        public async Task UpdateAsync(Department department)
        {
            if (await ExistsWithNameAsync(department.Name, department.DepartmentId))
            {
                // ⭐ English Error Message for Update
                throw new InvalidOperationException($"The department name '{department.Name}' already exists. Please choose a different name.");
            }
            await _repo.UpdateAsync(department);
        }
        public async Task<bool> ExistsWithNameAsync(string name, int? excludeId = null)
        {
            // Chỉ gọi qua Repository
            return await _repo.ExistsWithNameAsync(name, excludeId);
        }
        public async Task DeleteAsync(int id) => await _repo.SoftDeleteAsync(id); // Soft Delete
        public async Task RestoreAsync(int id) => await _repo.RestoreAsync(id); // Restore
    }
}
