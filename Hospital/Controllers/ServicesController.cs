using BLL.Services.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace Hospital.Controllers
{
    public class ServicesController : Controller
    {
        private readonly IServiceService _serviceService;

        public ServicesController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        // GET: Services
        public async Task<IActionResult> Index(string? searchString, int? page)
        {
            var services = await _serviceService.GetAllAsync();

            // Giữ lại chuỗi tìm kiếm để hiển thị
            ViewBag.SearchString = searchString;

            // Lọc theo tên nếu có nhập
            if (!string.IsNullOrEmpty(searchString))
            {
                services = services
                    .Where(s => s.Name != null &&
                                s.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Phân trang
            int pageSize = 10;
            int pageNumber = page ?? 1;

            return View(services.ToPagedList(pageNumber, pageSize));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service service)
        {
            // Kiểm tra bắt buộc
            if (string.IsNullOrWhiteSpace(service.Name))
                ModelState.AddModelError(nameof(service.Name), "Tên dịch vụ không được để trống.");

            if (string.IsNullOrWhiteSpace(service.Description))
                ModelState.AddModelError(nameof(service.Description), "Mô tả dịch vụ không được để trống.");

            if (service.Price <= 0)
                ModelState.AddModelError(nameof(service.Price), "Giá phải là số dương.");

            // Kiểm tra trùng tên (case-insensitive)
            var allServices = await _serviceService.GetAllAsync();
            bool nameExists = allServices.Any(s => s.Name.Trim().ToLower() == (service.Name ?? "").Trim().ToLower());
            if (nameExists)
                ModelState.AddModelError(nameof(service.Name), "Tên dịch vụ đã tồn tại.");

            if (!ModelState.IsValid)
                return View(service);

            await _serviceService.AddAsync(service);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var service = await _serviceService.GetByIdAsync(id);
            if (service == null) return NotFound();
            return View(service);
        }

        // POST: Services/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Service service)
        {
            if (id != service.ServiceId)
                return NotFound();

            // 🔹 Validate các trường bắt buộc
            if (string.IsNullOrWhiteSpace(service.Name))
                ModelState.AddModelError(nameof(service.Name), "Tên dịch vụ không được để trống.");

            if (string.IsNullOrWhiteSpace(service.Description))
                ModelState.AddModelError(nameof(service.Description), "Mô tả dịch vụ không được để trống.");

            if (service.Price <= 0)
                ModelState.AddModelError(nameof(service.Price), "Giá phải là số dương.");

            // 🔹 Kiểm tra trùng tên (an toàn với null)
            var allServices = await _serviceService.GetAllAsync();
            bool nameExists = allServices.Any(s =>
                s.ServiceId != service.ServiceId &&
                !string.IsNullOrWhiteSpace(s.Name) &&
                !string.IsNullOrWhiteSpace(service.Name) &&
                s.Name.Trim().ToLower() == service.Name.Trim().ToLower());

            if (nameExists)
                ModelState.AddModelError(nameof(service.Name), "Tên dịch vụ đã tồn tại.");

            // 🔹 Nếu có lỗi → trả lại View với ModelState
            if (!ModelState.IsValid)
                return View(service);

            // 🔹 Nếu hợp lệ → cập nhật
            await _serviceService.UpdateAsync(service);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var service = await _serviceService.GetByIdAsync(id);
            if (service == null) return NotFound();
            return View(service);
        }

        // DELETE (AJAX) - Services/DeleteConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _serviceService.GetByIdAsync(id);
            if (service == null)
                return NotFound();

            await _serviceService.DeleteAsync(id);
            return Ok(); // ✅ Cho fetch() biết là xóa thành công
        }
    }
}
