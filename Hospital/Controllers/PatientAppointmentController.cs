using BLL.Services;
using BLL.Services.Implements;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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


        public PatientAppointmentController(IDepartmentService departmentService, IDoctorService doctorService, IScheduleService scheduleService, IAppointmentService appointmentService, IServiceService serviceService)
        {
            _departmentService = departmentService;
            _doctorService = doctorService;
            _scheduleService = scheduleService;
            _appointmentService = appointmentService;
            _serviceService = serviceService;
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

            // Nếu là AJAX request → trả PartialView
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

            // Lọc bác sĩ
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

            // Nếu là AJAX request, trả về partial view
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("~/Views/Patients/_DoctorListPartial.cshtml", pagedDoctors);
            }

            return View("~/Views/Patients/SelectDoctor.cshtml", pagedDoctors);
        }


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

            Doctor doctor = await _doctorService.GetDoctorByIdAsync(doctorId.Value);

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
            ViewBag.ScheduleId = scheduleId;
            ViewBag.DoctorId = doctorId;
            ViewBag.DepartmentId = departmentId;

            // ✅ Lấy danh sách dịch vụ từ DB
            var services = await _serviceService.GetAllAsync();
            ViewBag.Services = services;

            // ✅ Lấy PatientId từ user đăng nhập
            var patientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(patientIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.PatientId = int.Parse(patientIdClaim);
            return View("~/Views/Patients/BookAppointment.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> BookAppointment(int scheduleId, int doctorId, int departmentId, int serviceId)
        {
            var patientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(patientIdClaim))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập trước khi đặt lịch.";
                return RedirectToAction("Login", "Account");
            }

            int patientId = int.Parse(patientIdClaim);

            var result = await _appointmentService.BookAppointmentAsync(scheduleId, doctorId, departmentId, patientId, serviceId);

            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Message;

            return RedirectToAction("Index");
        }


        public IActionResult AppointmentSuccess()
        {
            ViewBag.Message = TempData["SuccessMessage"];
            return View("~/Views/Patients/AppointmentSuccess.cshtml");
        }
    }
}