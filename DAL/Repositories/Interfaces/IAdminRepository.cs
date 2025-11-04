// DAL/Repositories/Interfaces/IAdminRepository.cs
using DAL.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<List<TopDoctorViewModel>> GetTopDoctorsByAppointmentsAsync(int top = 5);
        Task<List<MonthlyRevenueViewModel>> GetMonthlyRevenueAsync();
        Task<List<PatientCountViewModel>> GetPatientCountAsync(string type = "month");
    }
}