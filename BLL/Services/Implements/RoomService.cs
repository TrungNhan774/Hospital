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
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _repo;
        private readonly IDepartmentService _departmentService;

        public RoomService(IRoomRepository repo, IDepartmentService departmentService)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _departmentService = departmentService ?? throw new ArgumentNullException(nameof(departmentService));
        }

        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Room?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddAsync(Room room)
        {
            await _repo.AddAsync(room);
        }

        public async Task UpdateAsync(Room room)
        {
            await _repo.UpdateAsync(room);
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<Department>> GetDepartmentsAsync()
        => await _departmentService.GetAllAsync();
    }
}
