using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly DbhospitalContext _context;

        public DepartmentRepository(DbhospitalContext context)
        {
            _context = context;
        }

        public IEnumerable<Department> GetAll()
        {
            return _context.Departments
                .Include(d => d.Doctors)
                .Include(d => d.Rooms)
                .ToList();
        }

        public Department? GetById(int id)
        {
            return _context.Departments
                .Include(d => d.Doctors)
                .Include(d => d.Rooms)
                .FirstOrDefault(d => d.DepartmentId == id);
        }

        public void Add(Department department)
        {
            _context.Departments.Add(department);
            _context.SaveChanges();
        }

        public void Update(Department department)
        {
            _context.Departments.Update(department);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var dep = _context.Departments.Find(id);
            if (dep != null)
            {
                _context.Departments.Remove(dep);
                _context.SaveChanges();
            }
        }

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
