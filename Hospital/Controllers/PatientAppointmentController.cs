using BLL.Services;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Models.DTO;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IPatientService _patientService;
        private readonly IRoomService _roomService;
        private readonly IMedicalRecordService _medicalRecordService;

        public PatientAppointmentController(
            IDepartmentService departmentService,
            IDoctorService doctorService,
            IScheduleService scheduleService,
            IAppointmentService appointmentService,
            IServiceService serviceService,
            IPatientService patientService,
            IRoomService roomService,
            IMedicalRecordService medicalRecordService)
        {
            _departmentService = departmentService;
            _doctorService = doctorService;
            _scheduleService = scheduleService;
            _appointmentService = appointmentService;
            _serviceService = serviceService;
            _patientService = patientService;
            _roomService = roomService;
            _medicalRecordService = medicalRecordService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search, bool showDeleted = false, int page = 1, int pageSize = 10)
        {
            var allDepartments = await _departmentService.GetAllAsync(showDeleted);
            var departments = allDepartments.Select(d => new DepartmentDTO
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

            var pagedDepartments = departments.OrderBy(d => d.IsDeleted).ThenBy(d => d.Name).ToPagedList(page, pageSize);

            ViewBag.Search = search;
            ViewBag.ShowDeleted = showDeleted;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("~/Views/Patients/_DepartmentListPartial.cshtml", pagedDepartments);

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

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("~/Views/Patients/_DoctorListPartial.cshtml", pagedDoctors);

            return View("~/Views/Patients/SelectDoctor.cshtml", pagedDoctors);
        }

        [HttpGet]
        public async Task<IActionResult> SelectSchedule(int doctorId, int departmentId)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
            if (doctor == null)
                return NotFound($"Doctor with ID {doctorId} not found.");

            var schedules = await _scheduleService.GetAvailableSchedulesByDoctorIdAsync(doctorId);
            ViewBag.Doctorname = doctor.FullName;
            ViewBag.DoctorId = doctorId;
            ViewBag.DepartmentId = departmentId;

            if (schedules == null || !schedules.Any())
            {
                ViewBag.Message = $"Currently, there are no available schedules for Dr. {doctor.FullName}.";
                return View("~/Views/Patients/SelectSchedule.cshtml", new List<Schedule>());
            }

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

            ViewBag.Services = await _serviceService.GetAllAsync();
            ViewBag.Rooms = await _roomService.GetRoomsByDepartmentAsync(departmentId);

            var patientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(patientIdClaim))
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(patientIdClaim);
            var patient = await _patientService.GetPatientIdByUserIdAsync(userId);

            if (patient != null)
            {
                ViewBag.PatientId = patient.PatientId;
                ViewBag.HasPatient = true;
            }
            else
            {
                ViewBag.HasPatient = false;
            }
            Console.WriteLine($"BookAppointment called: {scheduleId}, {doctorId}, {departmentId}");
            return View("~/Views/Patients/BookAppointment.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> BookAppointment(int scheduleId, int doctorId, int departmentId, int serviceId, string? Notes, int? RoomId)
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
                string name = Request.Form["patient_name"];
                string gender = Request.Form["gender"];
                string address = Request.Form["address"];
                string phone = Request.Form["phone"];
                DateTime? dob = DateTime.TryParse(Request.Form["date_of_birth"], out var date) ? date : null;

                if (string.IsNullOrEmpty(name) || dob == null || string.IsNullOrEmpty(gender))
                {
                    TempData["ErrorMessage"] = "Please fill in patient information before booking.";
                    return RedirectToAction("BookAppointment", new { scheduleId, doctorId, departmentId });
                }

                var dto = new PatientDTO
                {
                    UserId = userId,
                    PatientName = name,
                    Address = address,
                    Phone = phone,
                    Gender = gender,
                    DateOfBirth = dob,
                    MedicalHistory = "",
                    IsDeleted = false
                };

                await _patientService.AddPatientAsync(dto);
                patient = await _patientService.GetPatientIdByUserIdAsync(userId);
            }

            var result = await _appointmentService.BookAppointmentAsync(scheduleId, doctorId, departmentId, patient.PatientId, serviceId, Notes, RoomId);

            if (result.Success)
            {
                await _medicalRecordService.AddMedicalRecordAsync(patient.PatientId, doctorId);
                TempData["SuccessMessage"] = result.Message;
            }
            else
                TempData["ErrorMessage"] = result.Message;

            return RedirectToAction("Index");
        }

   
        [HttpGet]
        public async Task<IActionResult> CheckScheduleAvailability(int scheduleId, int serviceId)
        {
            var (success, message) = await _appointmentService.CheckScheduleAvailabilityAsync(scheduleId, serviceId);
            return Json(new { success, message });
        }


        [HttpGet]
        public async Task<IActionResult> MyAppointments(string? status)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var patient = await _patientService.GetPatientIdByUserIdAsync(int.Parse(userId));
            if (patient == null)
                return BadRequest("No patient found for this user.");

            var appointments = await _appointmentService.GetAppointmentsByPatientAsync(patient.PatientId, status);
            ViewBag.SelectedStatus = status;
            return View("~/Views/Patients/MyAppointments.cshtml", appointments);
        }

        [HttpPost]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            bool success = await _appointmentService.CancelAppointmentAsync(appointmentId);
            if (!success)
                return NotFound();

            TempData["SuccessMessage"] = "Appointment cancellation successful.";
            return RedirectToAction("MyAppointments");
        }
    }
}
