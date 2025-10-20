using BLL.Services.Implements;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace Hospital.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsers = await _adminService.GetTotalUsersAsync();
            ViewBag.TotalDoctors = await _adminService.GetTotalDoctorsAsync();
            ViewBag.TotalCustomers = await _adminService.GetTotalCustomersAsync();

            return View();
        }

        public async Task<IActionResult> Users(string search, string sortOrder, int? page)
        {
            // 🔧 Thiết lập tham số sắp xếp
            ViewBag.CurrentSort = sortOrder;
            ViewBag.Search = search;

            // Các trạng thái sort
            ViewBag.FullNameSortParm = sortOrder == "fullname_asc" ? "fullname_desc" : "fullname_asc";
            ViewBag.UsernameSortParm = sortOrder == "username_asc" ? "username_desc" : "username_asc";

            // 📦 Lấy dữ liệu
            var users = await _adminService.GetAllAsync(search, sortOrder);

            // 📄 Phân trang
            int pageSize = 3;
            int pageNumber = page ?? 1;

            return View(users.ToPagedList(pageNumber, pageSize));
        }

    }
}