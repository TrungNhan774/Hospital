using BLL.DTOs;

namespace BLL.Services.Interfaces
{
    public interface IMedicalRecordService
    {
        Task<IEnumerable<PatientMedicalRecordDto>> GetRecordsByUserIdAsync(int userId);
        Task<PatientMedicalRecordDetailDto?> GetRecordDetailAsync(int recordId, int userId);
    }
}