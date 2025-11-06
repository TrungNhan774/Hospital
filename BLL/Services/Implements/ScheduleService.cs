using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Models.ViewModels;
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

        public async Task<int> BulkCreateAsync(CreateBulkScheduleViewModel model)
        {
            var schedulesToAdd = new List<Schedule>();
            var current = model.StartDate.ToDateTime(TimeOnly.MinValue);

            while (current.Date <= model.EndDate.ToDateTime(TimeOnly.MinValue).Date)
            {
                var dayName = current.DayOfWeek.ToString();
                if (model.SelectedDays.Contains(dayName))
                {
                    foreach (var shift in model.SelectedShifts)
                    {
                        var workDate = DateOnly.FromDateTime(current);
                        if (!await _repo.ExistsAsync(model.DoctorId, workDate, shift))
                        {
                            schedulesToAdd.Add(new Schedule
                            {
                                DoctorId = model.DoctorId,
                                WorkDate = workDate,
                                Shift = shift,
                                Available = true
                            });
                        }
                    }
                }
                current = current.AddDays(1);
            }

            if (schedulesToAdd.Any())
            {
                await _repo.AddRangeAsync(schedulesToAdd);
            }

            return schedulesToAdd.Count;
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _repo.ExistsAsync(id);
        }
    }
}
