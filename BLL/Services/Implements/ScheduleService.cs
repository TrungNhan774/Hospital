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
        private readonly DbhospitalContext _context;

        public ScheduleService(IScheduleRepository repo, DbhospitalContext context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<IEnumerable<Schedule>> GetAllAsync() => await _repo.GetAllAsync();
        public async Task<Schedule> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);
        public async Task AddAsync(Schedule schedule) => await _repo.AddAsync(schedule);
        public async Task UpdateAsync(Schedule schedule) => await _repo.UpdateAsync(schedule);
        public async Task DeleteAsync(int id) => await _repo.DeleteAsync(id);

        // Lấy danh sách bác sĩ (giống GetDepartmentsAsync bên RoomService)
        // Giống với GetDepartmentsAsync trong RoomService
        public async Task<IEnumerable<Doctor>> GetDoctorsAsync()
        {
            return await _context.Doctors
                .Include(d => d.Department)
                .Include(d => d.User)
                .ToListAsync();
        }
    }
}
