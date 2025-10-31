using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Models.DTO;
using DAL.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Implements
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorService(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync(string searchString = null)
        {
            return await _doctorRepository.GetAllAsync(searchString);
        }

        public async Task<Doctor> GetDoctorByIdAsync(int id)
        {
            return await _doctorRepository.GetByIdAsync(id);
        }

        public async Task CreateDoctorAsync(Doctor doctor)
        {
            await _doctorRepository.AddAsync(doctor);
        }

        public async Task UpdateDoctorAsync(Doctor doctor)
        {
            await _doctorRepository.UpdateAsync(doctor);
        }

        public async Task DeleteDoctorAsync(int id)
        {
            await _doctorRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsByDepartmentAsync(int? departmentId, string searchString = null)
        {
            return await _doctorRepository.GetByDepartmentAsync(departmentId, searchString);
        }

        public bool DoctorExists(int id)
        {
            return _doctorRepository.Exists(id);
        }
        public async Task<Doctor?> GetDoctorProfileByUserIdAsync(int userId)
        {
            return await _doctorRepository.GetDoctorProfileByUserIdAsync(userId);
        }
        public async Task<Doctor?> GetDoctorStatisticsAsync(int doctorId)
        {
            return await _doctorRepository.GetDoctorWithDetailsAsync(doctorId);
        
    }
        public async Task<IEnumerable<DoctorDTO>> GetDoctorsByDepartmentAsync(int departmentId)
        {
            // Lấy danh sách doctor theo department (dạng async)
            var doctors = await _doctorRepository.GetByDepartmentIdAsync(departmentId);

            // Chuyển sang DTO
            return doctors.Select(d => new DoctorDTO
            {
                DoctorId = d.DoctorId,
                FullName = d.FullName,
                Qualification = d.Qualification,
                ExperienceYears = d.ExperienceYears,
                About = d.About
            }).ToList();
        }
    }
}
