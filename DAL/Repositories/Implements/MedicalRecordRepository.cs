using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Implements
{
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly DbhospitalContext _context;

        public MedicalRecordRepository(DbhospitalContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId)
        {
            return await _context.MedicalRecords
                .AsNoTracking()
                .Include(r => r.Doctor)
                    .ThenInclude(d => d.User)
                .Where(r => r.PatientId == patientId)
                .OrderByDescending(r => r.CreatedAt ?? DateTime.MinValue)
                .ToListAsync();
        }

        public async Task<MedicalRecord?> GetDetailsByIdAsync(int recordId)
        {
            return await _context.MedicalRecords
                .AsNoTracking()
                .Include(r => r.Doctor)
                    .ThenInclude(d => d.User)
                .Include(r => r.MedicalRecordMedicines)
                    .ThenInclude(mrm => mrm.Medicine)
                .FirstOrDefaultAsync(r => r.RecordId == recordId);
        }
    }
}