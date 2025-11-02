using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Models.ViewModels;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BLL.Services.Implements
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly DbhospitalContext _context;
        public AdminService(IUserRepository userRepository, DbhospitalContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await _userRepository.CountAllAsync();
        }

        public async Task<int> GetTotalDoctorsAsync()
        {
            return await _userRepository.CountByRoleAsync("DOCTOR");
        }

        public async Task<int> GetTotalCustomersAsync()
        {
            return await _userRepository.CountByRoleAsync("CUSTOMER");
        }
        public async Task<List<User>> GetAllAsync(string search = null, string sortOrder = null)
        {
            return (List<User>)await _userRepository.GetAllAsync(search, sortOrder);
        }
        public async Task<List<TopDoctorViewModel>> GetTopDoctorsByAppointmentsAsync(int top = 5)
        {
            return await _context.Doctors
                .Select(d => new TopDoctorViewModel
                {
                    DoctorName = d.FullName,
                    AppointmentCount = d.Appointments.Count()
                })
                .OrderByDescending(d => d.AppointmentCount)
                .Take(top)
                .ToListAsync();
        }

        public async Task<List<MonthlyRevenueViewModel>> GetMonthlyRevenueAsync()
        {
            var payments = await _context.Payments
                .Where(p => p.Status == "CONFIRMED" && p.CreatedAt.HasValue)
                .GroupBy(p => new { Year = p.CreatedAt.Value.Year, Month = p.CreatedAt.Value.Month })
                .Select(g => new MonthlyRevenueViewModel
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalRevenue = g.Sum(p => p.TotalAmount)
                })
                .ToListAsync();

            var serviceRevenue = await _context.AppointmentServiceModels
                .Where(s => s.Appointment != null)
                .GroupBy(s => new { Year = s.Appointment.AppointmentDate.Year, Month = s.Appointment.AppointmentDate.Month })
                .Select(g => new MonthlyRevenueViewModel
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalRevenue = g.Sum(s => s.Service != null ? s.Service.Price : 0)
                })
                .ToListAsync();

            var result = payments
                .Concat(serviceRevenue)
                .GroupBy(x => new { x.Year, x.Month })
                .Select(g => new MonthlyRevenueViewModel
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalRevenue = g.Sum(x => x.TotalRevenue)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            return result;
        }


        public async Task<List<PatientCountViewModel>> GetPatientCountAsync(string type = "month")
        {
            if (type == "day")
            {
                var data = await _context.Appointments
                    .GroupBy(a => a.AppointmentDate.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        TotalPatients = g.Select(a => a.PatientId).Distinct().Count()
                    })
                    .OrderBy(g => g.Date)
                    .ToListAsync();

                return data.Select(g => new PatientCountViewModel
                {
                    Period = g.Date.ToString("yyyy-MM-dd"),
                    TotalPatients = g.TotalPatients
                }).ToList();
            }
            else
            {
                var data = await _context.Appointments
                    .GroupBy(a => new { a.AppointmentDate.Year, a.AppointmentDate.Month })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        TotalPatients = g.Select(a => a.PatientId).Distinct().Count()
                    })
                    .OrderBy(g => g.Year)
                    .ThenBy(g => g.Month)
                    .ToListAsync();

                // Format lại ở client-side
                return data.Select(g => new PatientCountViewModel
                {
                    Period = $"{g.Year}-{g.Month:D2}",
                    TotalPatients = g.TotalPatients
                }).ToList();
            }
        }

    }
}
