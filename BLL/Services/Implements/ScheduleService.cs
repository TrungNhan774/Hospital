using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implements
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _repo;
        private readonly IDoctorRepository _doctorRepository;

        public ScheduleService(IScheduleRepository repo, IDoctorRepository doctorRepository)
        {
            _repo = repo;
            _doctorRepository = doctorRepository;

        }

        public async Task<IEnumerable<Schedule>> GetAllAsync() => await _repo.GetAllAsync();
        public async Task<Schedule> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);
        public async Task AddAsync(Schedule schedule) => await _repo.AddAsync(schedule);
        public async Task UpdateAsync(Schedule schedule) => await _repo.UpdateAsync(schedule);
        public async Task DeleteAsync(int id) => await _repo.DeleteAsync(id);

        public async Task<IEnumerable<Doctor>> GetDoctorsAsync()
        => await _doctorRepository.GetAllAsync();
        public async Task<IEnumerable<Schedule>> GetAvailableSchedulesByDoctorIdAsync(int doctorId)
           => await _repo.GetAvailableSchedulesByDoctorIdAsync(doctorId);
    }
}
