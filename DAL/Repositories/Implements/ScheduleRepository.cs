using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Implements
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly DbhospitalContext _context;

        public ScheduleRepository(DbhospitalContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Schedule>> GetAllAsync()
        {
            return await _context.Schedules
                .Include(s => s.Doctor)
                .ToListAsync();
        }

        public async Task<Schedule> GetByIdAsync(int id)
        {
            return await _context.Schedules
                .Include(s => s.Doctor)
                .FirstOrDefaultAsync(s => s.ScheduleId == (int)id);
        }

        public async Task AddAsync(Schedule schedule)
        {
            await _context.Schedules.AddAsync(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Schedule schedule)
        {
            _context.Schedules.Update(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Schedule>> GetAvailableSchedulesByDoctorIdAsync(int doctorId)
        {
            // Lọc các ca làm việc chưa bị xóa và có 'available' = true từ hôm nay trở đi
            return await _context.Schedules
                .Where(s => s.DoctorId == doctorId && s.Available == true && s.WorkDate >= DateOnly.FromDateTime(DateTime.Today))
                .OrderBy(s => s.WorkDate)
                .ThenBy(s => s.Shift)
                .ToListAsync();
        }
    }
}
