using BLL.Services;
using BLL.Services.Implements;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace Hospital.Controllers
{
    [Authorize(Roles = "CUSTOMER")]
    public class PatientAppointmentController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly IDoctorService _doctorService;
        private readonly IScheduleService _scheduleService;
        private readonly IAppointmentService _appointmentService;
        private readonly IServiceService _serviceService;
        private readonly IPatientService _patientService;
        private readonly DbhospitalContext _context;
        private readonly IRoomService _roomService;

        public PatientAppointmentController(
            IDepartmentService departmentService,
            IDoctorService doctorService,
            IScheduleService scheduleService,
            IAppointmentService appointmentService,
            IServiceService serviceService,
            IPatientService patientService,
            DbhospitalContext context,
            IRoomService roomService)
        {
            _departmentService = departmentService;
            _doctorService = doctorService;
            _scheduleService = scheduleService;
            _appointmentService = appointmentService;
            _serviceService = serviceService;
            _patientService = patientService;
            _context = context;
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search, bool showDeleted = false, int page = 1, int pageSize = 10)
        {
            var allDepartments = await _departmentService.GetAllAsync(showDeleted);

            IEnumerable<DepartmentDTO> departments = allDepartments.Select(d => new DepartmentDTO
            {
                DepartmentId = d.DepartmentId,
                Name = d.Name,
                Description = d.Description,
                DoctorCount = d.Doctors?.Count ?? 0,
                RoomCount = d.Rooms?.Count ?? 0,
                IsDeleted = d.IsDeleted
            });

            // 🔹 Search filter
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

            // If AJAX request → return partial view
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("~/Views/Patients/_DepartmentListPartial.cshtml", pagedDepartments);
            }

            return View("~/Views/Patients/PatientAppointment.cshtml", pagedDepartments);
        }

        [HttpGet]
        public async Task<IActionResult> SelectDoctor(int departmentId, string search = "", int page = 1, int pageSize = 5)
        {
            var department = await _departmentService.GetByIdAsync(departmentId);
            if (department == null)
                return NotFound();

            ViewBag.DepartmentName = department.Name;
            ViewBag.DepartmentId = departmentId;
            ViewBag.Search = search;

            // 🔹 Filter doctors
            var doctors = await _doctorService.GetDoctorsByDepartmentAsync(departmentId);

            if (!string.IsNullOrEmpty(search))
            {
                string s = search.Trim().ToLowerInvariant();
                doctors = doctors.Where(d =>
                    d.FullName.ToLowerInvariant().Contains(s) ||
                    d.Qualification.ToLowerInvariant().Contains(s)
                ).ToList();
            }

            var pagedDoctors = doctors.OrderBy(d => d.FullName).ToPagedList(page, pageSize);

            // If AJAX → return partial
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("~/Views/Patients/_DoctorListPartial.cshtml", pagedDoctors);
            }

            return View("~/Views/Patients/SelectDoctor.cshtml", pagedDoctors);
        }

        [HttpGet]
        public async Task<IActionResult> SelectSchedule(int? doctorId, int? departmentId)
        {
            if (doctorId == null || doctorId.Value <= 0)
            {
                return BadRequest("Doctor ID is missing or invalid.");
            }
            if (departmentId == null || departmentId.Value <= 0)
            {
                return BadRequest("Department ID is missing or invalid.");
            }

            var doctor = await _doctorService.GetDoctorByIdAsync(doctorId.Value);
            if (doctor == null)
            {
                return NotFound($"Doctor with ID {doctorId.Value} not found.");
            }

            var schedules = await _scheduleService.GetAvailableSchedulesByDoctorIdAsync(doctorId.Value);
            if (schedules == null || !schedules.Any())
            {
                ViewBag.Message = $"Currently, there are no available schedules for Dr. {doctor.FullName}.";
                ViewBag.Doctorname = doctor.FullName;
                ViewBag.DoctorId = doctorId.Value;
                ViewBag.DepartmentId = departmentId.Value;
                return View("~/Views/Patients/SelectSchedule.cshtml", new List<Schedule>());
            }

            ViewBag.Doctorname = doctor.FullName;
            ViewBag.DoctorId = doctorId.Value;
            ViewBag.DepartmentId = departmentId.Value;

            return View("~/Views/Patients/SelectSchedule.cshtml", schedules);
        }

        [HttpGet]
        public async Task<IActionResult> BookAppointment(int scheduleId, int doctorId, int departmentId)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
            ViewBag.DoctorName = doctor.FullName;

            var department = await _departmentService.GetByIdAsync(departmentId);
            ViewBag.DepartmentName = department.Name;

            ViewBag.ScheduleId = scheduleId;
            ViewBag.DoctorId = doctorId;
            ViewBag.DepartmentId = departmentId;

            // ✅ Load Services
            var services = await _serviceService.GetAllAsync();
            ViewBag.Services = services;

            // ✅ Load Rooms theo Department
            var rooms = await _roomService.GetRoomsByDepartmentAsync(departmentId);
            ViewBag.Rooms = rooms;

            // ✅ Lấy PatientId theo user đang đăng nhập
            var patientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(patientIdClaim))
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(patientIdClaim);
            var patient = await _patientService.GetPatientIdByUserIdAsync(userId);
            if (patient == null)
                return BadRequest("No patient found for this user.");

            ViewBag.PatientId = patient.PatientId;

            return View("~/Views/Patients/BookAppointment.cshtml");
        }


        [HttpPost]
        public async Task<IActionResult> BookAppointment(
    int scheduleId, int doctorId, int departmentId, int serviceId, string? Notes, int? RoomId)
        {
            var patientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(patientIdClaim))
            {
                TempData["ErrorMessage"] = "Please log in before booking an appointment.";
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(patientIdClaim);
            var patient = await _patientService.GetPatientIdByUserIdAsync(userId);

            if (patient == null)
            {
                return BadRequest("No patient found for this user.");
            }

            var result = await _appointmentService.BookAppointmentAsync(
                scheduleId, doctorId, departmentId, patient.PatientId, serviceId, Notes, RoomId);

            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Message;

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> CheckScheduleAvailability(int scheduleId, int serviceId)
        {
            var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);
            if (schedule == null)
                return Json(new { success = false, message = "Schedule not found." });

            var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == serviceId && s.IsActive);
            if (service == null)
                return Json(new { success = false, message = "Service not found or inactive." });

            int newWeight = (schedule.Weight ?? 0) + service.Weight;
            if (newWeight > 3)
                return Json(new { success = false, message = "The schedule is full, please choose another slot." });

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> MyAppointments(string? status)
        {
            // ✅ Lấy UserId từ token đăng nhập
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // ✅ Tìm PatientId tương ứng
            var patient = await _patientService.GetPatientIdByUserIdAsync(int.Parse(userId));
            if (patient == null)
            {
                return BadRequest("No patient found for this user.");
            }

            // ✅ Lấy danh sách Appointment của bệnh nhân
            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Room)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(apService => apService.Service)
                .Where(a => a.PatientId == patient.PatientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            // ✅ Lọc theo trạng thái nếu có
            if (!string.IsNullOrEmpty(status))
            {
                appointments = appointments.Where(a => a.Status == status).ToList();
            }

            ViewBag.SelectedStatus = status;

            return View("~/Views/Patients/MyAppointments.cshtml", appointments);
        }


        [HttpPost]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
                return NotFound();

            appointment.Status = "CANCELLED";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Appointment cancellation successful.";
            return RedirectToAction("MyAppointments");
        }

    }
}
