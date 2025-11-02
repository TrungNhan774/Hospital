using BLL.DTOs;
using DAL.Models;
using DAL.Models.DTO;
using System.Collections.Generic;

namespace BLL.Services
{
    public interface IPatientService
    {
        IEnumerable<Patient> GetAll();
        Patient GetById(int id);
        void Add(Patient patient);
        void Update(Patient patient);
        void Delete(int id);
        IEnumerable<User> GetAllUsers(); // để hiển thị dropdown chọn user
        Task<Patient?> GetByIdDAsync(int id);
        Task UpdateAsync(Patient patient);
        Task<IEnumerable<Patient>> GetAllAsync(bool showDeleted = false);
        Task<Patient> GetByIdAsync(int id);
        Task AddAsync(Patient patient);
        Task DeleteAsync(int id);
        Task<PatientIdDto?> GetPatientIdByUserIdAsync(int userId);
       Task AddPatientAsync(PatientDTO dto);
    }
}
