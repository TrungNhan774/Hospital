using DAL.Models;
using DAL.Models.ViewModels;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IAdminService
    {
        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalDoctorsAsync();
        Task<int> GetTotalCustomersAsync();
        Task<List<User>> GetAllAsync(string search = null, string sortOrder = null);
        Task<List<TopDoctorViewModel>> GetTopDoctorsByAppointmentsAsync(int top = 5);
        Task<List<MonthlyRevenueViewModel>> GetMonthlyRevenueAsync();
        Task<List<PatientCountViewModel>> GetPatientCountAsync(string type = "month");
    }
}
