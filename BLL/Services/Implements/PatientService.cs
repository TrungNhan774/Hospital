using DAL.Models;
using DAL.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepo;
        private readonly DbhospitalContext _context;

        public PatientService(IPatientRepository patientRepo, DbhospitalContext context)
        {
            _patientRepo = patientRepo;
            _context = context;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync(bool showDeleted = false) => await _patientRepo.GetAllAsync(showDeleted);
        public async Task<Patient> GetByIdAsync(int id) => await _patientRepo.GetByIdAsync(id);
        public async Task AddAsync(Patient patient) => await _patientRepo.AddAsync(patient);
        public async Task UpdateAsync(Patient patient) => await _patientRepo.UpdateAsync(patient);
        public async Task DeleteAsync(int id) => await _patientRepo.SoftDeleteAsync(id);

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }
    }
}