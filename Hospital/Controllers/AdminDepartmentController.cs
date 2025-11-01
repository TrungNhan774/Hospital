using BLL.Services;
using DAL.Models;
using DAL.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;
using System;

namespace Hospital.Controllers
{
    [Route("Admin/[controller]")]
    public class AdminDepartmentController : Controller
    {
        private readonly IDepartmentService _service;

        public AdminDepartmentController(IDepartmentService service)
        {
            _service = service;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string search, bool showDeleted = false, int page = 1, int pageSize = 10)
        {
            var allDepartments = await _service.GetAllAsync(showDeleted);

            IEnumerable<DepartmentDTO> departments = allDepartments.Select(d => new DepartmentDTO
            {
                DepartmentId = d.DepartmentId,
                Name = d.Name,
                Description = d.Description,
                DoctorCount = d.Doctors?.Count ?? 0,
                RoomCount = d.Rooms?.Count ?? 0,
                IsDeleted = d.IsDeleted
            });

            if (!string.IsNullOrEmpty(search))
            {
                string normalizedSearch = search.Trim().ToLowerInvariant();
                departments = departments.Where(d =>
                    d.Name.ToLowerInvariant().Contains(normalizedSearch) ||
                    (d.Description != null && d.Description.ToLowerInvariant().Contains(normalizedSearch))
                );
            }

            var pagedDepartments = departments
                .OrderBy(d => d.IsDeleted)
                .ThenBy(d => d.Name)
                .ToPagedList(page, pageSize);

            ViewBag.Search = search;
            ViewBag.ShowDeleted = showDeleted;

            return View("~/Views/Admin/Departments/Index.cshtml", pagedDepartments);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Departments/Create.cshtml", new DepartmentDTO());
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentDTO dto)
        {
            if (ModelState.IsValid)
            {
                var dep = new Department
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    IsDeleted = false
                };

                try
                {
                    await _service.AddAsync(dep);
                    TempData["Success"] = "✅ Department created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("Name", ex.Message);
                }
            }

            TempData["Error"] = "⚠️ Please fix the errors in the form.";
            return View("~/Views/Admin/Departments/Create.cshtml", dto);
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var dep = await _service.GetByIdAsync(id);
            if (dep == null)
            {
                TempData["Error"] = "❌ Department not found.";
                return RedirectToAction(nameof(Index));
            }

            var dto = new DepartmentDTO
            {
                DepartmentId = dep.DepartmentId,
                Name = dep.Name,
                Description = dep.Description
            };

            return View("~/Views/Admin/Departments/Edit.cshtml", dto);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DepartmentDTO dto)
        {
            if (id != dto.DepartmentId)
            {
                TempData["Error"] = "❌ Department ID mismatch.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                var dep = await _service.GetByIdAsync(id);
                if (dep == null)
                {
                    TempData["Error"] = "❌ Department not found.";
                    return RedirectToAction(nameof(Index));
                }

                dep.Name = dto.Name;
                dep.Description = dto.Description;

                try
                {
                    await _service.UpdateAsync(dep);
                    TempData["Success"] = "✅ Department updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("Name", ex.Message);
                }
            }

            TempData["Error"] = "⚠️ Please fix the errors in the form.";
            return View("~/Views/Admin/Departments/Edit.cshtml", dto);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var dep = await _service.GetByIdAsync(id);
            if (dep == null)
            {
                TempData["Error"] = "❌ Department not found.";
                return RedirectToAction(nameof(Index));
            }

            var dto = new DepartmentDTO
            {
                DepartmentId = dep.DepartmentId,
                Name = dep.Name,
                Description = dep.Description,
                DoctorCount = dep.Doctors?.Count ?? 0,
                RoomCount = dep.Rooms?.Count ?? 0,
                IsDeleted = dep.IsDeleted
            };

            return View("~/Views/Admin/Departments/Details.cshtml", dto);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, bool showDeleted = false, string search = null, int page = 1)
        {
            await _service.DeleteAsync(id);

            TempData["Success"] = $"🗑️ Department soft deleted successfully!";
            return RedirectToAction(nameof(Index), new { showDeleted, search, page });
        }

        [HttpPost("Restore/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id, bool showDeleted = true, string search = null, int page = 1)
        {
            await _service.RestoreAsync(id);

            TempData["Success"] = $"♻️ Department restored successfully!";
            return RedirectToAction(nameof(Index), new { showDeleted = true, search, page });
        }
    }
}