using DAL.Models;

namespace DAL.Repositories.Interfaces
{
    public interface IMedicalRecordRepository
    {
        Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId);
        Task<MedicalRecord?> GetDetailsByIdAsync(int recordId);
    }
}