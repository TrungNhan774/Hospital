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
    }
}
