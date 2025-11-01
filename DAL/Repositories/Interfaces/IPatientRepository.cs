using BLL.DTOs;
using DAL.Models;
using DAL.Models.DTO;
using System.Collections.Generic;

namespace DAL.Repositories
{
    public interface IPatientRepository
    {
        IEnumerable<Patient> GetAll();
        Patient GetById(int id);
        void Add(Patient patient);
        void Update(Patient patient);
        void Delete(int id);
        Task<Patient?> GetByIdDAsync(int id);
        Task UpdateAsync(Patient patient);
        Task<IEnumerable<Patient>> GetAllAsync(bool showDeleted = false);
        Task<Patient> GetByIdAsync(int id);
        Task AddAsync(Patient patient);
        //Task UpdateAsync(Patient patient);
        Task SoftDeleteAsync(int id);
        Task RestoreAsync(int id);
        Task HardDeleteAsync(int id);
        Task<PatientIdDto?> GetPatientIdByUserIdAsync(int userId);
    }
}
