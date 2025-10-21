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

        // Lấy danh sách User (chỉ dành cho dropdown)
        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }
    }
}
