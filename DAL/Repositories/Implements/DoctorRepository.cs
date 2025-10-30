using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Implements
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly DbhospitalContext _context;

        public DoctorRepository(DbhospitalContext context)
        {
            _context = context;
        }

        // 🔹 Lấy tất cả bác sĩ đang hoạt động
        public async Task<IEnumerable<Doctor>> GetAllAsync(string searchString = null)
        {
            var doctors = _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .Where(d => d.IsActive) // ✅ Chỉ lấy bác sĩ còn hoạt động
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                doctors = doctors.Where(d =>
                    d.FullName.Contains(searchString) ||
                    d.Specialization.Contains(searchString) ||
                    d.Department.Name.Contains(searchString));
            }

            return await doctors.OrderBy(d => d.DoctorId).ToListAsync();
        }

        // 🔹 Lấy 1 bác sĩ đang hoạt động
        public async Task<Doctor> GetByIdAsync(int id)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.DoctorId == id && d.IsActive);
        }

        public async Task AddAsync(Doctor doctor)
        {
            doctor.IsActive = true; // ✅ Bác sĩ mới luôn active
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();
        }

        // 🔹 Xóa mềm thay vì xóa thật
        public async Task DeleteAsync(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                doctor.IsActive = false; // ✅ Soft delete
                _context.Doctors.Update(doctor);
                await _context.SaveChangesAsync();
            }
        }

        public Task<Doctor?> GetByUserIdAsync(int userId)
        {
            return _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
        }
        public bool Exists(int id)
        {
            return _context.Doctors.Any(d => d.DoctorId == id && d.IsActive);
        }

        public async Task<IEnumerable<Doctor>> GetByDepartmentAsync(int? departmentId, string searchString = null)
        {
            var doctors = _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .AsQueryable();

            if (departmentId.HasValue)
            {
                doctors = doctors.Where(d => d.DepartmentId == departmentId.Value);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                doctors = doctors.Where(d =>
                    d.FullName.Contains(searchString) ||
                    d.Specialization.Contains(searchString) ||
                    d.Department.Name.Contains(searchString));
            }

            return await doctors.OrderBy(d => d.DoctorId).ToListAsync();
        }
        public async Task<Doctor?> GetDoctorProfileByUserIdAsync(int userId)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.UserId == userId && d.IsActive);
        }
        public async Task<Doctor?> GetDoctorWithDetailsAsync(int doctorId)
        {
            return await _context.Doctors
                .Include(d => d.Appointments)
                .Include(d => d.MedicalRecords)
                .FirstOrDefaultAsync(d => d.DoctorId == doctorId);
        }

    }
}
