using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Implements
{
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly DbhospitalContext _context;

        public MedicalRecordRepository(DbhospitalContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MedicalRecord>> GetByDoctorAsync(int doctorId)
        {
            return await _context.MedicalRecords
                .Where(r => r.DoctorId == doctorId)
                .Include(r => r.Patient)
                    .ThenInclude(p => p.User) // 👈 thêm dòng này để lấy tên
                .Include(r => r.MedicalRecordMedicines)
                    .ThenInclude(mrm => mrm.Medicine)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }


        public async Task<MedicalRecord?> GetByIdAndDoctorAsync(int recordId, int doctorId)
        {
            return await _context.MedicalRecords
                .Where(r => r.RecordId == recordId && r.DoctorId == doctorId)
                .Include(r => r.Patient)
                    .ThenInclude(p => p.User) // 👈 thêm dòng này
                .Include(r => r.MedicalRecordMedicines)
                    .ThenInclude(mrm => mrm.Medicine)
                .FirstOrDefaultAsync();
        }

        // 🧩 Admin xem toàn bộ
        public async Task<IEnumerable<MedicalRecord>> GetAllAsync()
        {
            return await _context.MedicalRecords
                .Include(r => r.Patient).ThenInclude(p => p.User)
                .Include(r => r.Doctor).ThenInclude(d => d.User)
                .Include(r => r.MedicalRecordMedicines).ThenInclude(mrm => mrm.Medicine)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        // 🧩 Admin xem chi tiết theo recordId
        public async Task<MedicalRecord?> GetByIdAsync(int recordId)
        {
            return await _context.MedicalRecords
                .Where(r => r.RecordId == recordId)
                .Include(r => r.Patient).ThenInclude(p => p.User)
                .Include(r => r.Doctor).ThenInclude(d => d.User)
                .Include(r => r.MedicalRecordMedicines).ThenInclude(mrm => mrm.Medicine)
                .FirstOrDefaultAsync();
        }
        public async Task<MedicalRecord?> GetWithDetailsAsync(int id)
        {
            return await _context.MedicalRecords
                .Include(r => r.Patient).ThenInclude(p => p.User)
                .Include(r => r.MedicalRecordMedicines).ThenInclude(m => m.Medicine)
                .FirstOrDefaultAsync(r => r.RecordId == id);
        }

        public async Task UpdateAsync(MedicalRecord record)
        {
            _context.MedicalRecords.Update(record);
            await _context.SaveChangesAsync();
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
        public async Task AddAsync(MedicalRecord record)
        {
            await _context.MedicalRecords.AddAsync(record);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
