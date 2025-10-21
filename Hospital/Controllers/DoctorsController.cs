using BLL.Services.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace Hospital.Controllers
{
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
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var doctors = await _doctorService.GetAllDoctorsAsync(searchString);
            ViewBag.SearchString = searchString;

            return View(doctors.ToPagedList(pageNumber, pageSize));
        }

        // GET: Doctors/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        // GET: Doctors/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username");
            return View();
        }

        // POST: Doctors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Doctor doctor)
        {
            if (!ModelState.IsValid)
            {
                ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", doctor.DepartmentId);
                ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username", doctor.UserId);
                return View(doctor);
            }

            try
            {
                // Kiểm tra trùng Email
                if (!string.IsNullOrEmpty(doctor.Email) &&
                    await _context.Doctors.AnyAsync(d => d.Email == doctor.Email))
                {
                    ModelState.AddModelError("Email", "This email already exists.");
                    ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", doctor.DepartmentId);
                    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username", doctor.UserId);
                    return View(doctor);
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
            return View(doctor);
        }

        // GET: Doctors/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound();

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", doctor.DepartmentId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username", doctor.UserId);
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Doctor doctor)
        {
            if (id != doctor.DoctorId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", doctor.DepartmentId);
                ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username", doctor.UserId);
                return View(doctor);
            }

            try
            {
                // Kiểm tra trùng email (trừ chính nó)
                if (!string.IsNullOrEmpty(doctor.Email) &&
                    await _context.Doctors.AnyAsync(d => d.Email == doctor.Email && d.DoctorId != doctor.DoctorId))
                {
                    ModelState.AddModelError("Email", "This email already exists.");
                    return View(doctor);
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
            return View(doctor);
        }

        // GET: Doctors/Delete/5
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _doctorService.DeleteDoctorAsync(id);
                TempData["SuccessMessage"] = "Doctor deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting doctor: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
