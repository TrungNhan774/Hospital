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
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }
        [Route("")]
        // GET: Appointments
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var appointments = await _appointmentService.GetAllAsync(searchString);
            ViewBag.SearchString = searchString;
            return View("~/Views/Admin/Appointments/Index.cshtml", appointments.ToPagedList(pageNumber, pageSize));
        }

        // GET: Appointments/Details/5
        [Route("Details/{id?}")]
        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);
            if (appointment == null)
                return NotFound();

            return View("~/Views/Admin/Appointments/Details.cshtml", appointment);
        }

        //// GET: Appointments/Create
        //public IActionResult Create()
        //{
        //    ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Address");
        //    ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Address");
        //    ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber");
        //    return View();
        //}

        //// POST: Appointments/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("AppointmentId,PatientId,DoctorId,RoomId,AppointmentDate,Status,Notes")] Appointment appointment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(appointment);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Address", appointment.DoctorId);
        //    ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Address", appointment.PatientId);
        //    ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber", appointment.RoomId);
        //    return View(appointment);
        //}

        //// GET: Appointments/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var appointment = await _context.Appointments.FindAsync(id);
        //    if (appointment == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Address", appointment.DoctorId);
        //    ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Address", appointment.PatientId);
        //    ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber", appointment.RoomId);
        //    return View(appointment);
        //}

        //// POST: Appointments/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,PatientId,DoctorId,RoomId,AppointmentDate,Status,Notes")] Appointment appointment)
        //{
        //    if (id != appointment.AppointmentId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(appointment);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!AppointmentExists(appointment.AppointmentId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Address", appointment.DoctorId);
        //    ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Address", appointment.PatientId);
        //    ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber", appointment.RoomId);
        //    return View(appointment);
        //}

        //// GET: Appointments/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var appointment = await _context.Appointments
        //        .Include(a => a.Doctor)
        //        .Include(a => a.Patient)
        //        .Include(a => a.Room)
        //        .FirstOrDefaultAsync(m => m.AppointmentId == id);
        //    if (appointment == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(appointment);
        //}

        //// POST: Appointments/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var appointment = await _context.Appointments.FindAsync(id);
        //    if (appointment != null)
        //    {
        //        _context.Appointments.Remove(appointment);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool AppointmentExists(int id)
        //{
        //    return _context.Appointments.Any(e => e.AppointmentId == id);
        //}
    }
}
