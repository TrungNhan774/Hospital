using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories;
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
        private readonly IPatientRepository _patientRepo;

        public MedicalRecordService(IMedicalRecordRepository repo, IPatientRepository patientRepository)
        {
            _repo = repo;
            _patientRepo = patientRepository;
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
        public async Task<MedicalRecord?> GetByIdAsync(int id)
        {
            return await _repo.GetWithDetailsAsync(id);
        }

        public async Task UpdateAsync(MedicalRecord record)
        {
            await _repo.UpdateAsync(record);
        }
        public async Task<MedicalRecord?> GetRecordWithMedicinesForDoctorAsync(int recordId, int doctorId)
        {
            return await _repo.GetByIdAndDoctorAsync(recordId, doctorId);

        public async Task<IEnumerable<PatientMedicalRecordDto>> GetRecordsByUserIdAsync(int userId)
        {
            var patientIdDto = await _patientRepo.GetPatientIdByUserIdAsync(userId);

            if (patientIdDto == null)
            {
                return new List<PatientMedicalRecordDto>();
            }

            var records = await _repo.GetByPatientIdAsync(patientIdDto.PatientId);

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

            var record = await _repo.GetDetailsByIdAsync(recordId);

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
