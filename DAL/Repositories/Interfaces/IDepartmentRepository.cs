

using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks; // ⭐ Cần dùng Task

namespace DAL.Repositories
{
    public interface IDepartmentRepository
    {
        // 1. Lấy tất cả (Async + Thêm tùy chọn showDeleted)
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