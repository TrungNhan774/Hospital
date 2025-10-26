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
    }
}
