using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IMedicalRecordRepository
    {
        Task<IEnumerable<MedicalRecord>> GetByDoctorAsync(int doctorId);
        Task<MedicalRecord?> GetByIdAndDoctorAsync(int recordId, int doctorId);
        // ➕ Thêm 2 method cho Admin:
        Task<IEnumerable<MedicalRecord>> GetAllAsync();
        Task<MedicalRecord?> GetByIdAsync(int recordId);
        Task<MedicalRecord?> GetWithDetailsAsync(int id);
        Task UpdateAsync(MedicalRecord record);
        Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId);
        Task<MedicalRecord?> GetDetailsByIdAsync(int recordId);
    }
}
