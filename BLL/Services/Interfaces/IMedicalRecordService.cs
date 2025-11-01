using BLL.DTOs;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IMedicalRecordService
    {
        Task<IEnumerable<MedicalRecord>> GetRecordsForDoctorAsync(int doctorId);
        Task<MedicalRecord?> GetRecordDetailsForDoctorAsync(int recordId, int doctorId);
        // ➕ Thêm method cho Admin:
        Task<IEnumerable<MedicalRecord>> GetAllRecordsAsync();
        Task<MedicalRecord?> GetRecordDetailsAsync(int recordId);
        Task<MedicalRecord?> GetByIdAsync(int id);  // thêm dòng này
        Task UpdateAsync(MedicalRecord record);
        Task<MedicalRecord?> GetRecordWithMedicinesForDoctorAsync(int recordId, int doctorId);


        Task<IEnumerable<PatientMedicalRecordDto>> GetRecordsByUserIdAsync(int userId);
        Task<PatientMedicalRecordDetailDto?> GetRecordDetailAsync(int recordId, int userId);
    }
}
