using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implements
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _repo;

        public MedicalRecordService(IMedicalRecordRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<MedicalRecord>> GetRecordsForDoctorAsync(int doctorId)
        {
            return _repo.GetByDoctorAsync(doctorId);
        }

        public Task<MedicalRecord?> GetRecordDetailsForDoctorAsync(int recordId, int doctorId)
        {
            return _repo.GetByIdAndDoctorAsync(recordId, doctorId);
        }

        // 🧩 Cho Admin:
        public Task<IEnumerable<MedicalRecord>> GetAllRecordsAsync()
            => _repo.GetAllAsync();

        public Task<MedicalRecord?> GetRecordDetailsAsync(int recordId)
            => _repo.GetByIdAsync(recordId);
    }
}
