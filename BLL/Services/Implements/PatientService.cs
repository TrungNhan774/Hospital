using DAL.Models;
using DAL.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepo;
        private readonly DbhospitalContext _context; // dùng để lấy Users (chỉ trong service)

        public PatientService()
        {
            _patientRepo = new PatientRepository();
            _context = new DbhospitalContext();
        }

        // CRUD
        public IEnumerable<Patient> GetAll() => _patientRepo.GetAll();
        public Patient GetById(int id) => _patientRepo.GetById(id);
        public void Add(Patient patient) => _patientRepo.Add(patient);
        public void Update(Patient patient) => _patientRepo.Update(patient);
        public void Delete(int id) => _patientRepo.Delete(id);

        public async Task<IEnumerable<Patient>> GetAllAsync(bool showDeleted = false) => await _patientRepo.GetAllAsync(showDeleted);
        public async Task<Patient> GetByIdAsync(int id) => await _patientRepo.GetByIdAsync(id);
        public async Task AddAsync(Patient patient) => await _patientRepo.AddAsync(patient);
        public async Task UpdateAsync(Patient patient) => await _patientRepo.UpdateAsync(patient);
        public async Task DeleteAsync(int id) => await _patientRepo.SoftDeleteAsync(id);

        // Lấy danh sách User (chỉ dành cho dropdown)
        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }
    }
}
