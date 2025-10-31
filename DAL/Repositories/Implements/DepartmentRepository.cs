using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    // ⭐ GIẢ ĐỊNH IDepartmentRepository ĐÃ ĐƯỢC CẬP NHẬT TƯƠNG ỨNG VỚI ExistsWithNameAsync
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly DbhospitalContext _context;

        public DepartmentRepository(DbhospitalContext context)
        {
            _context = context;
        }

        // 1. GetAllAsync 
        public async Task<IEnumerable<Department>> GetAllAsync(bool showDeleted = false)
        {
            var query = _context.Departments
                .Include(d => d.Doctors)
                .Include(d => d.Rooms)
                .AsQueryable();

            if (!showDeleted)
                query = query.Where(d => !d.IsDeleted);

            return await query.OrderBy(d => d.Name).ToListAsync();
        }

        // 2. GetByIdAsync
        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.Doctors)
                .Include(d => d.Rooms)
                .FirstOrDefaultAsync(d => d.DepartmentId == id);
        }

        // 3. AddAsync 
        public async Task AddAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
        }

        // 4. UpdateAsync 
        public async Task UpdateAsync(Department department)
        {
            _context.Entry(department).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // 5. SoftDeleteAsync 
        public async Task SoftDeleteAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                department.IsDeleted = true;
                _context.Entry(department).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        // 6. RestoreAsync 
        public async Task RestoreAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                department.IsDeleted = false;
                _context.Entry(department).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        // 7. HardDeleteAsync
        public async Task HardDeleteAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsWithNameAsync(string name, int? excludeId = null)
        {
            string normalizedName = name.Trim().ToLower();

            var query = _context.Departments.AsQueryable();

            query = query.Where(d => d.Name.ToLower() == normalizedName);
            if (excludeId.HasValue)
            {
                query = query.Where(d => d.DepartmentId != excludeId.Value);
            }
            return await query.AnyAsync();
        }
    }
}