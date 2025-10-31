using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Repositories;
using DAL.Repositories.Interfaces;

namespace BLL.Services.Implements
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _medicalRecordRepo;
        private readonly IPatientRepository _patientRepo;

        public MedicalRecordService(IMedicalRecordRepository medicalRecordRepo, IPatientRepository patientRepo)
        {
            _medicalRecordRepo = medicalRecordRepo;
            _patientRepo = patientRepo;
        }

        public async Task<IEnumerable<PatientMedicalRecordDto>> GetRecordsByUserIdAsync(int userId)
        {
            var patientIdDto = await _patientRepo.GetPatientIdByUserIdAsync(userId);

            if (patientIdDto == null)
            {
                return new List<PatientMedicalRecordDto>();
            }

            var records = await _medicalRecordRepo.GetByPatientIdAsync(patientIdDto.PatientId);

            var dtos = records.Select(r => new PatientMedicalRecordDto
            {
                RecordId = r.RecordId,
                CreatedAt = r.CreatedAt,
                Diagnosis = r.Diagnosis,
                Prescription = r.Prescription,
                DoctorName = r.Doctor?.User?.FullName ?? r.Doctor.FullName ?? "N/A"
            }).ToList();

            return dtos;
        }

        public async Task<PatientMedicalRecordDetailDto?> GetRecordDetailAsync(int recordId, int userId)
        {
            var patientIdDto = await _patientRepo.GetPatientIdByUserIdAsync(userId);
            if (patientIdDto == null) return null;

            var record = await _medicalRecordRepo.GetDetailsByIdAsync(recordId);

            if (record == null || record.PatientId != patientIdDto.PatientId)
            {
                return null;
            }

            var detailDto = new PatientMedicalRecordDetailDto
            {
                RecordId = record.RecordId,
                CreatedAt = record.CreatedAt,
                Diagnosis = record.Diagnosis,
                Prescription = record.Prescription,
                DoctorName = record.Doctor?.User?.FullName ?? record.Doctor.FullName ?? "N/A",
                PrescribedMedicines = record.MedicalRecordMedicines.Select(mrm => new MedicineDto
                {
                    Name = mrm.Medicine.Name,
                    Unit = mrm.Medicine.Unit,
                    Dosage = mrm.Dosage,
                    Quantity = mrm.Quantity ?? 0
                }).ToList()
            };

            return detailDto;
        }
    }
}