using DAL.Models;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IAdminService
    {
        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalDoctorsAsync();
        Task<int> GetTotalCustomersAsync();
        Task<List<User>> GetAllAsync(string search = null, string sortOrder = null);
    }
}
