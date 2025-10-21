using BLL.Services.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace Hospital.Controllers
{
    public class MedicinesController : Controller
    {
        private readonly IMedicineService _medicineService;

        public MedicinesController(IMedicineService medicineService)
        {
            _medicineService = medicineService;
        }

        // GET: Medicines
        public async Task<IActionResult> Index(string? searchString, int? page) //phân trang
        {
            var medicines = await _medicineService.GetAllAsync();

            // Lưu chuỗi tìm kiếm để hiển thị lại trên view
            ViewBag.SearchString = searchString;

            // Nếu có từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                medicines = medicines
                    .Where(m => m.Name != null &&
                                m.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Thiết lập phân trang
            int pageSize = 10;
            int pageNumber = page ?? 1;

            // Trả về view có search và phân trang hoạt động cùng nhau
            return View(medicines.ToPagedList(pageNumber, pageSize));
        }

        // GET: Medicines/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Medicines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Medicine medicine)
        {
            if (string.IsNullOrWhiteSpace(medicine.Name))
                ModelState.AddModelError(nameof(medicine.Name), "Drug name cannot be left blank.");

            if (string.IsNullOrWhiteSpace(medicine.Unit))
                ModelState.AddModelError(nameof(medicine.Unit), "The medication unit cannot be left blank.");

            if (string.IsNullOrWhiteSpace(medicine.Description))
                ModelState.AddModelError(nameof(medicine.Description), "Drug description cannot be left blank.");

            if (medicine.Price <= 0)
                ModelState.AddModelError(nameof(medicine.Price), "Drug price must be positive.");

            var allMedicines = await _medicineService.GetAllAsync();
            bool nameExists = allMedicines.Any(m => m.Name.Trim().ToLower() == (medicine.Name ?? "").Trim().ToLower());
            if (nameExists)
                ModelState.AddModelError(nameof(medicine.Name), "The drug name already exists.");

            if (!ModelState.IsValid)
                return View(medicine);

            await _medicineService.AddAsync(medicine);
            return RedirectToAction(nameof(Index));
        }

        // GET: Medicines/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var medicine = await _medicineService.GetByIdAsync(id);
            if (medicine == null)
                return NotFound();

            return View(medicine);
        }

        // POST: Medicines/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Medicine medicine)
        {
            if (id != medicine.MedicineId)
                return NotFound();

            // 🔹 Validate các trường bắt buộc
            if (string.IsNullOrWhiteSpace(medicine.Name))
                ModelState.AddModelError(nameof(medicine.Name), "Drug name cannot be left blank.");

            if (string.IsNullOrWhiteSpace(medicine.Unit))
                ModelState.AddModelError(nameof(medicine.Unit), "The medication unit cannot be left blank.");

            if (string.IsNullOrWhiteSpace(medicine.Description))
                ModelState.AddModelError(nameof(medicine.Description), "Drug description cannot be left blank.");

            if (medicine.Price <= 0)
                ModelState.AddModelError(nameof(medicine.Price), "Drug price must be positive.");

            // 🔹 Kiểm tra trùng tên (an toàn với null)
            var allMedicines = await _medicineService.GetAllAsync();
            bool nameExists = allMedicines.Any(m =>
                m.MedicineId != medicine.MedicineId &&
                !string.IsNullOrWhiteSpace(m.Name) &&
                !string.IsNullOrWhiteSpace(medicine.Name) &&
                m.Name.Trim().ToLower() == medicine.Name.Trim().ToLower());

            if (nameExists)
                ModelState.AddModelError(nameof(medicine.Name), "The drug name already exists.");

            // 🔹 Nếu có lỗi → trả lại View với ModelState
            if (!ModelState.IsValid)
                return View(medicine);

            // 🔹 Nếu hợp lệ → cập nhật
            await _medicineService.UpdateAsync(medicine);
            return RedirectToAction(nameof(Index));
        }

        // GET: Medicines/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var medicine = await _medicineService.GetByIdAsync(id);
            if (medicine == null)
                return NotFound();

            return View(medicine);
        }

        // POST: Medicines/DeleteConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _medicineService.DeleteAsync(id);
            return Ok(); // ✅ cho fetch() nhận biết xóa thành công
        }
    }
}