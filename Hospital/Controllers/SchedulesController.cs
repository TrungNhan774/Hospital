using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace Hospital.Controllers
{
    [Route("Admin/[controller]")]
    [Authorize(Roles = "ADMIN")]
    public class SchedulesController : Controller
    {
        private readonly IScheduleService _scheduleService;
        private readonly DbhospitalContext _context;

        public SchedulesController(IScheduleService scheduleService, DbhospitalContext context)
        {
            _scheduleService = scheduleService;
            _context = context;
        }

        // GET: Admin/Schedules
        [Route("")]
        public async Task<IActionResult> Index(string? search, int? page)
        {
            var schedules = await _scheduleService.GetAllAsync();

            // Keep the search string to display again on View
            ViewBag.Search = search;

            // Filter by doctor's name or work date
            if (!string.IsNullOrEmpty(search))
            {
                schedules = schedules
                    .Where(s =>
                        (s.Doctor != null && s.Doctor.User != null &&
                         s.Doctor.User.FullName.Contains(search, StringComparison.OrdinalIgnoreCase))
                        || s.WorkDate.ToString("yyyy-MM-dd").Contains(search))
                    .ToList();
            }

            // Pagination
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var pagedSchedules = schedules.ToPagedList(pageNumber, pageSize);

            return View("~/Views/Admin/Schedules/Index.cshtml", pagedSchedules);
        }

        // GET: Admin/Schedules/Details/5
        [Route("Details/{id?}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var schedule = await _scheduleService.GetByIdAsync(id.Value);
            if (schedule == null)
                return NotFound();

            return View("~/Views/Admin/Schedules/Details.cshtml", schedule);
        }

        // GET: Admin/Schedules/Edit/5
        [Route("Edit/{id?}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var schedule = await _scheduleService.GetByIdAsync(id.Value);
            if (schedule == null)
                return NotFound();

            ViewData["DoctorId"] = new SelectList(await _scheduleService.GetDoctorsAsync(), "DoctorId", "User.FullName", schedule.DoctorId);
            return View("~/Views/Admin/Schedules/Edit.cshtml", schedule);
        }

        // POST: Admin/Schedules/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, [Bind("ScheduleId,DoctorId,WorkDate,Shift,Available")] Schedule schedule)
        {
            if (id != schedule.ScheduleId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _scheduleService.UpdateAsync(schedule);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ScheduleExists(id))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewData["DoctorId"] = new SelectList(await _scheduleService.GetDoctorsAsync(), "DoctorId", "User.FullName", schedule.DoctorId);
            return View("~/Views/Admin/Schedules/Edit.cshtml", schedule);
        }

        // GET: Admin/Schedules/Delete/5
        [Route("Delete/{id?}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var schedule = await _scheduleService.GetByIdAsync(id.Value);
            if (schedule == null)
                return NotFound();

            return View("~/Views/Admin/Schedules/Delete.cshtml", schedule);
        }

        // POST: Admin/Schedules/DeleteConfirmed/5 (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DeleteConfirmed/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _scheduleService.GetByIdAsync(id);
            if (schedule == null)
                return NotFound();

            await _scheduleService.DeleteAsync(id);
            return Ok();
        }

        private async Task<bool> ScheduleExists(int id)
        {
            var schedule = await _scheduleService.GetByIdAsync(id);
            return schedule != null;
        }

        [HttpGet]
        [Route("CreateBulk")]
        public async Task<IActionResult> CreateBulk()
        {
            ViewBag.Doctors = new SelectList(
                await _scheduleService.GetDoctorsAsync(),
                "DoctorId",
                "User.FullName"
            );
            var today = DateOnly.FromDateTime(DateTime.Today);
            var model = new CreateBulkScheduleViewModel
            {
                SelectedShifts = new List<string>(),
                SelectedDays = new List<string>(),
                StartDate = today,
                EndDate = today.AddDays(6)
            };

            return View("~/Views/Admin/Schedules/CreateBulk.cshtml", model);
        }

        [HttpPost]
        [Route("CreateBulk")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBulk(CreateBulkScheduleViewModel model)
        {
            // 1. Validate dates
            if (model.StartDate > model.EndDate)
                ModelState.AddModelError("EndDate", "End date must be later than start date!");

            // 2. Validate doctor
            if (model.DoctorId <= 0)
                ModelState.AddModelError("DoctorId", "Please select a doctor!");

            // 3. Validate shifts
            if (model.SelectedShifts == null || !model.SelectedShifts.Any())
                ModelState.AddModelError("SelectedShifts", "Please select at least one work shift!");

            // 4. Validate days of the week
            if (model.SelectedDays == null || !model.SelectedDays.Any())
                ModelState.AddModelError("SelectedDays", "Please select at least one day of the week!");

            // If validation fails → return to form with messages
            if (!ModelState.IsValid)
            {
                ViewBag.Doctors = new SelectList(await _scheduleService.GetDoctorsAsync(), "DoctorId", "User.FullName");
                return View("~/Views/Admin/Schedules/CreateBulk.cshtml", model);
            }

            // --- CREATE SCHEDULES ---
            var schedulesToAdd = new List<Schedule>();
            var current = model.StartDate.ToDateTime(TimeOnly.MinValue);

            while (current.Date <= model.EndDate.ToDateTime(TimeOnly.MinValue).Date)
            {
                var dayName = current.DayOfWeek.ToString(); // "Monday", ...

                if (model.SelectedDays.Contains(dayName))
                {
                    foreach (var shift in model.SelectedShifts)
                    {
                        var workDateOnly = DateOnly.FromDateTime(current);

                        bool exists = await _context.Schedules.AnyAsync(s =>
                            s.DoctorId == model.DoctorId &&
                            s.WorkDate == workDateOnly &&
                            s.Shift == shift);

                        if (!exists)
                        {
                            schedulesToAdd.Add(new Schedule
                            {
                                DoctorId = model.DoctorId,
                                WorkDate = workDateOnly,
                                Shift = shift,
                                Available = true
                            });
                        }
                    }
                }
                current = current.AddDays(1);
            }

            if (schedulesToAdd.Any())
            {
                await _context.Schedules.AddRangeAsync(schedulesToAdd);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Successfully created {schedulesToAdd.Count} work schedules!";
            }
            else
            {
                TempData["Info"] = "No new schedules were created (either already existed or no valid days).";
            }

            return RedirectToAction("Index");
        }
    }
}
