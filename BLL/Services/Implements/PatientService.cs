using BLL.DTOs;
using DAL.Models;
using DAL.Models.DTO;
using DAL.Repositories;
using DAL.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepo;
        private readonly IUserRepository _userRepo;
        public PatientService(IPatientRepository patientRepo, IUserRepository userRepo)
        {
            _patientRepo = patientRepo;
            _userRepo = userRepo;
        }

        public IEnumerable<Patient> GetAll()
        {
            // gọi hàm GetAllAsync().Result nếu repo chỉ có async
            return _patientRepo.GetAllAsync(false).Result;
        }

        public Patient GetById(int id)
        {
            return _patientRepo.GetByIdAsync(id).Result;
        }

        public void Add(Patient patient)
        {
            _patientRepo.AddAsync(patient).Wait();
        }

        public void Update(Patient patient)
        {
            _patientRepo.UpdateAsync(patient).Wait();
        }

        public void Delete(int id)
        {
            _patientRepo.SoftDeleteAsync(id).Wait();
        }

        public async Task<IEnumerable<Patient>> GetAllAsync(bool showDeleted = false)
            => await _patientRepo.GetAllAsync(showDeleted);

        public async Task<Patient?> GetByIdAsync(int id)
            => await _patientRepo.GetByIdAsync(id);

        public async Task AddAsync(Patient patient)
            => await _patientRepo.AddAsync(patient);

        public async Task UpdateAsync(Patient patient)
            => await _patientRepo.UpdateAsync(patient);

        public async Task DeleteAsync(int id)
            => await _patientRepo.SoftDeleteAsync(id);

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = await _userRepo.GetAllAsync();
            return users;
        }

        public async Task<Patient?> GetByIdDAsync(int id)
            => await _patientRepo.GetByIdDAsync(id);

        public async Task<PatientIdDto?> GetPatientIdByUserIdAsync(int userId)
            => await _patientRepo.GetPatientIdByUserIdAsync(userId);

        public async Task AddPatientAsync(PatientDTO dto)
        {
            var patient = new Patient
            {
                UserId = dto.UserId,
                PatientName = dto.PatientName,
                Phone = dto.Phone,
                Gender = dto.Gender,
                Address = dto.Address,
                DateOfBirth = dto.DateOfBirth,
                MedicalHistory = dto.MedicalHistory,
                IsDeleted = dto.IsDeleted
            };

            await _patientRepo.AddAsync(patient);
        }

        public IEnumerable<User> GetAllUsers()
          => _userRepo.GetAll();

    }
}
