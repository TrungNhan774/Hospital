using BLL.DTOs;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly DbhospitalContext _context;

        public PatientRepository()
        {
            _context = new DbhospitalContext();
        }

        public async Task<IEnumerable<Patient>> GetAllAsync(bool showDeleted = false)
        {
            var query = _context.Patients
                .Include(p => p.User)
                .AsQueryable();

            if (!showDeleted)
                query = query.Where(p => !p.IsDeleted);

            return await query.ToListAsync();
        }

        public async Task<Patient> GetByIdAsync(int id)
        {
            return await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PatientId == id);
        }

        public async Task AddAsync(Patient patient)
        {
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Patient patient)
        {
            _context.Entry(patient).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                patient.IsDeleted = true;
                _context.Entry(patient).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RestoreAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                patient.IsDeleted = false;
                _context.Entry(patient).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task HardDeleteAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PatientIdDto?> GetPatientIdByUserIdAsync(int userId)
        {
            var patient = await _context.Patients
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null) return null;

            return new PatientIdDto { PatientId = patient.PatientId };
        }
    }
}