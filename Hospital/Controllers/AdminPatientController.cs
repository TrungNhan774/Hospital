using BLL.Services;
using DAL.Models;
using DAL.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System.Text;
using X.PagedList;
using X.PagedList.Extensions;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.Controllers
{
    [Route("Admin/[Controller]")]
    public class AdminPatientController : Controller
    {
        private readonly IPatientService _patientService;

        public AdminPatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string search, string gender, bool showDeleted = false, int page = 1, int pageSize = 10)
        {
            var allPatients = await _patientService.GetAllAsync(showDeleted);

            IEnumerable<PatientDTO> patients = allPatients.Select(p => new PatientDTO
            {
                PatientId = p.PatientId,
                UserId = p.UserId,
                UserFullName = p.User?.FullName,
                PatientName = p.PatientName,
                Phone = p.Phone,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender,
                Address = p.Address,
                MedicalHistory = p.MedicalHistory,
                IsDeleted = p.IsDeleted
            });

            static string NormalizeString(string input)
            {
                if (string.IsNullOrWhiteSpace(input))
                    return string.Empty;

                input = input.Trim().ToLowerInvariant();
                var normalized = input.Normalize(NormalizationForm.FormD);
                var sb = new StringBuilder();
                foreach (var c in normalized)
                {
                    if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                        sb.Append(c);
                }
                return sb.ToString().Normalize(NormalizationForm.FormC);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var normalizedSearch = NormalizeString(search);
                patients = patients.Where(p =>
                    (!string.IsNullOrEmpty(p.PatientName) && NormalizeString(p.PatientName).Contains(normalizedSearch)) ||
                    (!string.IsNullOrEmpty(p.UserFullName) && NormalizeString(p.UserFullName).Contains(normalizedSearch)) ||
                    (!string.IsNullOrEmpty(p.Phone) && p.Phone.Contains(search)) ||
                    (!string.IsNullOrEmpty(p.Address) && NormalizeString(p.Address).Contains(normalizedSearch))
                );
            }

            if (!string.IsNullOrEmpty(gender))
            {
                patients = patients.Where(p => p.Gender.Equals(gender, StringComparison.OrdinalIgnoreCase));
            }

            var pagedPatients = patients
                .OrderBy(p => p.PatientName)
                .ToPagedList(page, pageSize);

            ViewBag.Search = search;
            ViewBag.Gender = gender;
            ViewBag.ShowDeleted = showDeleted;

            return View("~/Views/Admin/Patients/Index.cshtml", pagedPatients);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewBag.Users = new SelectList(_patientService.GetAllUsers(), "UserId", "FullName");
            return View("~/Views/Admin/Patients/Create.cshtml", new PatientDTO());
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientDTO dto)
        {
            if (ModelState.IsValid)
            {
                var patient = new Patient
                {
                    UserId = dto.UserId,
                    PatientName = dto.PatientName,
                    Phone = dto.Phone,
                    DateOfBirth = dto.DateOfBirth,
                    Gender = dto.Gender,
                    Address = dto.Address,
                    MedicalHistory = dto.MedicalHistory,
                    IsDeleted = false
                };

                await _patientService.AddAsync(patient);
                TempData["Success"] = "✅ Patient created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Users = new SelectList(_patientService.GetAllUsers(), "UserId", "FullName", dto.UserId);
            TempData["Error"] = "⚠️ Please fix the errors in the form.";
            return View("~/Views/Admin/Patients/Create.cshtml", dto);
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            if (patient == null) return NotFound();

            var dto = new PatientDTO
            {
                PatientId = patient.PatientId,
                UserId = patient.UserId,
                UserFullName = patient.User?.FullName,
                PatientName = patient.PatientName,
                Phone = patient.Phone,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Address = patient.Address,
                MedicalHistory = patient.MedicalHistory,
                IsDeleted = patient.IsDeleted
            };

            ViewBag.Users = new SelectList(_patientService.GetAllUsers(), "UserId", "FullName", dto.UserId);
            return View("~/Views/Admin/Patients/Edit.cshtml", dto);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PatientDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Users = new SelectList(_patientService.GetAllUsers(), "UserId", "FullName", dto.UserId);
                TempData["Error"] = "⚠️ Please fix the errors in the form.";
                return View("~/Views/Admin/Patients/Edit.cshtml", dto);
            }

            var patient = await _patientService.GetByIdAsync(id);
            if (patient == null) return NotFound();

            patient.UserId = dto.UserId;
            patient.PatientName = dto.PatientName;
            patient.Phone = dto.Phone;
            patient.DateOfBirth = dto.DateOfBirth;
            patient.Gender = dto.Gender;
            patient.Address = dto.Address;
            patient.MedicalHistory = dto.MedicalHistory;

            await _patientService.UpdateAsync(patient);
            TempData["Success"] = "✅ Patient updated successfully!";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, bool showDeleted = false, string search = null, string gender = null, int page = 1)
        {
            var patient = await _patientService.GetByIdAsync(id);
            if (patient == null) return NotFound();

            patient.IsDeleted = true;
            await _patientService.UpdateAsync(patient);

            TempData["Success"] = "🗑️ Patient soft deleted successfully!";
            return RedirectToAction(nameof(Index), new { showDeleted, search, gender, page });
        }

        [HttpPost("Restore/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id, bool showDeleted = false, string search = null, string gender = null, int page = 1)
        {
            var patient = await _patientService.GetByIdAsync(id);
            if (patient == null) return NotFound();

            patient.IsDeleted = false;
            await _patientService.UpdateAsync(patient);

            TempData["Success"] = "♻️ Patient restored successfully!";
            return RedirectToAction(nameof(Index), new { showDeleted, search, gender, page });
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            if (patient == null) return NotFound();

            var dto = new PatientDTO
            {
                PatientId = patient.PatientId,
                UserId = patient.UserId,
                UserFullName = patient.User?.FullName,
                PatientName = patient.PatientName,
                Phone = patient.Phone,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Address = patient.Address,
                MedicalHistory = patient.MedicalHistory,
                IsDeleted = patient.IsDeleted
            };

            return View("~/Views/Admin/Patients/Details.cshtml", dto);
        }

        [HttpGet("ToggleDeleted")]
        public IActionResult ToggleDeleted(bool showDeleted, string search, string gender, int page = 1)
        {
            return RedirectToAction(nameof(Index), new { showDeleted, search, gender, page });
        }
    }
}