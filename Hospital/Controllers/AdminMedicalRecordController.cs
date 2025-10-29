using BLL.Services.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace Hospital.Controllers
{
    public class AdminMedicalRecordController : Controller
    {
        private readonly IMedicalRecordService _medicalRecordService;

        public AdminMedicalRecordController(IMedicalRecordService medicalRecordService)
        {
            _medicalRecordService = medicalRecordService;
        }

        // 📋 Hiển thị danh sách bệnh án (hỗ trợ tìm kiếm + phân trang)
        public async Task<IActionResult> Index(string? searchString, int? page)
        {
            // Lấy toàn bộ hồ sơ bệnh án
            var records = await _medicalRecordService.GetAllRecordsAsync();

            // Lưu chuỗi tìm kiếm để hiển thị lại trên View
            ViewBag.SearchString = searchString;

            // Nếu có từ khóa tìm kiếm -> lọc theo tên bệnh nhân hoặc bác sĩ hoặc chẩn đoán
            if (!string.IsNullOrEmpty(searchString))
            {
                records = records
                    .Where(r =>
                        (r.Patient?.User?.FullName != null &&
                         r.Patient.User.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                        (r.Doctor?.User?.FullName != null &&
                         r.Doctor.User.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                        (r.Diagnosis != null &&
                         r.Diagnosis.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            // Thiết lập phân trang
            int pageSize = 10;
            int pageNumber = page ?? 1;

            // Trả về view có search + phân trang hoạt động cùng nhau
            return View(records.ToPagedList(pageNumber, pageSize));
        }

        // 🔍 Xem chi tiết bệnh án
        public async Task<IActionResult> Details(int id)
        {
            var record = await _medicalRecordService.GetRecordDetailsAsync(id);
            if (record == null)
                return NotFound();

            return View(record);
        }
    }
}
