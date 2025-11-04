using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetAllAsync();
        Task<Room?> GetByIdAsync(int id);
        Task AddAsync(Room room);
        Task UpdateAsync(Room room);
        Task DeleteAsync(int id);
        Task<IEnumerable<Department>> GetDepartmentsAsync();
        Task<IEnumerable<Room>> GetRoomsByDepartmentAsync(int departmentId);
    }
}
