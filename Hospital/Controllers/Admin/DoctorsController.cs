using BLL.Services.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace Hospital.Controllers.Admin
{
    [Route("Admin/[controller]")]
    [Authorize(Roles = "ADMIN")]
    public class DoctorsController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly DbhospitalContext _context;

        public DoctorsController(IDoctorService doctorService, DbhospitalContext context)
        {
            _doctorService = doctorService;
            _context = context;
        }

        // GET: Doctors
        [Route("")]
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var doctors = await _doctorService.GetAllDoctorsAsync(searchString);
            ViewBag.SearchString = searchString;

            return View("~/Views/Admin/Doctors/Index.cshtml", doctors.ToPagedList(pageNumber, pageSize));
        }

        // GET: Doctors/Details/5
        [Route("Details/{id?}")]
        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound();

            return View("~/Views/Admin/Doctors/Details.cshtml", doctor);
        }

        // GET: Doctors/Create
        [Route("Create")]
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name");
            ViewData["UserId"] = new SelectList(
             _context.Users
                 .Where(u => u.IsActive
                     && u.Role == "DOCTOR"
                     && !_context.Doctors.Any(d => d.UserId == u.UserId && d.IsActive)),
             "UserId",
             "Username"
         );
            return View("~/Views/Admin/Doctors/Create.cshtml");
        }

        // POST: Doctors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(Doctor doctor)
        {
            if (!ModelState.IsValid)
            {
                ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", doctor.DepartmentId);
                ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username");
                return View("~/Views/Admin/Doctors/Create.cshtml", doctor);
            }

            try
            {
                // Kiểm tra trùng Email (chỉ với bác sĩ còn hoạt động)
                if (!string.IsNullOrEmpty(doctor.Email) &&
                    await _context.Doctors.AnyAsync(d => d.Email == doctor.Email && d.IsActive))
                {
                    ModelState.AddModelError("Email", "This email already exists.");
                    ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", doctor.DepartmentId);
                    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username", doctor.UserId);
                    return View("~/Views/Admin/Doctors/Create.cshtml", doctor);
                }


                await _doctorService.CreateDoctorAsync(doctor);
                TempData["SuccessMessage"] = "Doctor created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", $"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Unexpected error: {ex.Message}");
            }

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", doctor.DepartmentId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username", doctor.UserId);
            return View("~/Views/Admin/Doctors/Create.cshtml", doctor);
        }

        // GET: Doctors/Edit/5
        [Route("Edit/{id?}")]
        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound();

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", doctor.DepartmentId);

            ViewData["UserId"] = new SelectList(
                _context.Users.Where(u => u.IsActive
                    && u.Role == "DOCTOR"
                    && (
                        !_context.Doctors.Any(d => d.UserId == u.UserId && d.IsActive)
                        || u.UserId == doctor.UserId
                    )),
                "UserId",
                "Username",
                doctor.UserId
            );

            return View("~/Views/Admin/Doctors/Edit.cshtml", doctor);
        }

        // POST: Doctors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, Doctor doctor)
        {
            if (id != doctor.DoctorId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", doctor.DepartmentId);
                ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username", doctor.UserId);
                return View("~/Views/Admin/Doctors/Edit.cshtml", doctor);
            }

            try
            {
                // Kiểm tra trùng email (trừ chính nó)
                if (!string.IsNullOrEmpty(doctor.Email) &&
                    await _context.Doctors.AnyAsync(d => d.Email == doctor.Email && d.DoctorId != doctor.DoctorId && d.IsActive))
                {
                    ModelState.AddModelError("Email", "This email already exists.");
                    return View("~/Views/Admin/Doctors/Edit.cshtml", doctor);
                }


                await _doctorService.UpdateDoctorAsync(doctor);
                TempData["SuccessMessage"] = "Doctor updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_doctorService.DoctorExists(doctor.DoctorId))
                    return NotFound();
                else
                    ModelState.AddModelError("", "The record was updated by another user. Please reload.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Unexpected error: {ex.Message}");
            }

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", doctor.DepartmentId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username", doctor.UserId);
            return View("~/Views/Admin/Doctors/Edit.cshtml", doctor);
        }

        // GET: Doctors/Delete/5
        [Route("Delete/{id?}")]
        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("DeleteConfirmed/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _doctorService.DeleteDoctorAsync(id);
                TempData["SuccessMessage"] = "Doctor deleted successfully.";
                return Ok(); // Cho fetch() nhận biết xoá thành công
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting doctor: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
