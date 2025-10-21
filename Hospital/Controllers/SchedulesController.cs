using BLL.Services.Interfaces;
using DAL.Models;
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

        public SchedulesController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        // GET: Admin/Schedules
        [Route("")]
        public async Task<IActionResult> Index(string? search, int? page)
        {
            var schedules = await _scheduleService.GetAllAsync();

            // Giữ lại chuỗi tìm kiếm để hiển thị lại trên View
            ViewBag.Search = search;

            // Lọc theo tên bác sĩ hoặc ngày làm việc
            if (!string.IsNullOrEmpty(search))
            {
                schedules = schedules
                    .Where(s =>
                        (s.Doctor != null && s.Doctor.User != null &&
                         s.Doctor.User.FullName.Contains(search, StringComparison.OrdinalIgnoreCase))
                        || s.WorkDate.ToString("yyyy-MM-dd").Contains(search))
                    .ToList();
            }

            // Phân trang
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

        // GET: Admin/Schedules/Create
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            ViewData["DoctorId"] = new SelectList(await _scheduleService.GetDoctorsAsync(), "DoctorId", "User.FullName");
            return View("~/Views/Admin/Schedules/Create.cshtml");
        }

        // POST: Admin/Schedules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create([Bind("ScheduleId,DoctorId,WorkDate,Shift,Available")] Schedule schedule)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _scheduleService.AddAsync(schedule);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving schedule: " + ex.Message);
                }
            }

            ViewData["DoctorId"] = new SelectList(await _scheduleService.GetDoctorsAsync(), "DoctorId", "User.FullName", schedule.DoctorId);
            return View("~/Views/Admin/Schedules/Create.cshtml", schedule);
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
    }
}
