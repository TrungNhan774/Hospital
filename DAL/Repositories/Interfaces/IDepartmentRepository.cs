using DAL.Models;
using System.Collections.Generic;

namespace DAL.Repositories
{
    public interface IDepartmentRepository
    {
        IEnumerable<Department> GetAll();
        Department? GetById(int id);
        void Add(Department department);
        void Update(Department department);
        void Delete(int id);
        Task<IEnumerable<Department>> GetAllAsync(bool showDeleted = false);

        // 2. Lấy theo ID (Async)
        Task<Department?> GetByIdAsync(int id);

        // 3. Thêm (Async)
        Task AddAsync(Department department);

        // 4. Cập nhật (Async)
        Task UpdateAsync(Department department);

        // 5. Xóa Mềm (Soft Delete)
        Task SoftDeleteAsync(int id);

        // 6. Khôi Phục (Restore)
        Task RestoreAsync(int id);

        // 7. Xóa Vĩnh Viễn (Hard Delete - Tùy chọn, để hoàn chỉnh)
        Task HardDeleteAsync(int id);
        Task<bool> ExistsWithNameAsync(string name, int? excludeId = null);
    }
}
