using BLL.Services;
using DAL.Models;
using DAL.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System.Text;
using X.PagedList;
using X.PagedList.Extensions;

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
        public IActionResult Index(string search, string gender, int page = 1, int pageSize = 10)
        {
            // Lấy danh sách DTO
            var patients = _patientService.GetAll()
                .Select(p => new PatientDTO
                {
                    PatientId = p.PatientId,
                    UserId = p.UserId,
                    UserFullName = p.User?.FullName,
                    DateOfBirth = p.DateOfBirth,
                    Gender = p.Gender,
                    Address = p.Address,
                    MedicalHistory = p.MedicalHistory
                });

            // Hàm chuẩn hóa chuỗi: loại dấu, trim, lowercase
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

            // Tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                var normalizedSearch = NormalizeString(search);

                patients = patients.Where(p =>
                    (!string.IsNullOrEmpty(p.UserFullName) && NormalizeString(p.UserFullName).Contains(normalizedSearch)) ||
                    (!string.IsNullOrEmpty(p.Address) && NormalizeString(p.Address).Contains(normalizedSearch))
                );
            }

            // Lọc theo giới tính
            if (!string.IsNullOrEmpty(gender))
            {
                patients = patients.Where(p => p.Gender.Equals(gender, StringComparison.OrdinalIgnoreCase));
            }

            // Sắp xếp và phân trang
            var pagedPatients = patients
                .OrderBy(p => p.UserFullName)
                .ToPagedList(page, pageSize);

            ViewBag.Search = search;
            ViewBag.Gender = gender;

            return View("~/Views/Admin/Patients/Index.cshtml", pagedPatients);
        }


        // ✅ GET: /Admin/Patients/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewBag.Users = new SelectList(_patientService.GetAllUsers(), "UserId", "FullName");
            return View("~/Views/Admin/Patients/Create.cshtml", new PatientDTO());
        }

        // ✅ POST: /Admin/Patients/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PatientDTO dto)
        {
            if (ModelState.IsValid)
            {
                var patient = new Patient
                {
                    UserId = dto.UserId,
                    DateOfBirth = dto.DateOfBirth,
                    Gender = dto.Gender,
                    Address = dto.Address,
                    MedicalHistory = dto.MedicalHistory
                };

                _patientService.Add(patient);
                TempData["Success"] = "✅ Patient created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Users = new SelectList(_patientService.GetAllUsers(), "UserId", "FullName", dto.UserId);
            TempData["Error"] = "⚠️ Please fix the errors in the form.";
            return View("~/Views/Admin/Patients/Create.cshtml", dto);
        }

        // ✅ GET: /Admin/Patients/Edit/5
        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var patient = _patientService.GetById(id);
            if (patient == null)
                return NotFound();

            var dto = new PatientDTO
            {
                PatientId = patient.PatientId,
                UserId = patient.UserId,
                UserFullName = patient.User?.FullName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Address = patient.Address,
                MedicalHistory = patient.MedicalHistory
            };

            ViewBag.Users = new SelectList(_patientService.GetAllUsers(), "UserId", "FullName", dto.UserId);
            return View("~/Views/Admin/Patients/Edit.cshtml", dto);
        }

        // ✅ POST: /Admin/Patients/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PatientDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Users = new SelectList(_patientService.GetAllUsers(), "UserId", "FullName", dto.UserId);
                TempData["Error"] = "⚠️ Please fix the errors in the form.";
                return View("~/Views/Admin/Patients/Edit.cshtml", dto);
            }

            var patient = new Patient
            {
                PatientId = dto.PatientId,
                UserId = dto.UserId,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                Address = dto.Address,
                MedicalHistory = dto.MedicalHistory
            };

            _patientService.Update(patient);
            TempData["Success"] = "✅ Patient updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ✅ GET: /Admin/Patients/Delete/5
        [HttpGet("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var patient = _patientService.GetById(id);
            if (patient == null)
                return NotFound();

            var dto = new PatientDTO
            {
                PatientId = patient.PatientId,
                UserId = patient.UserId,
                UserFullName = patient.User?.FullName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Address = patient.Address,
                MedicalHistory = patient.MedicalHistory
            };

            return View("~/Views/Admin/Patients/Delete.cshtml", dto);
        }

        // ✅ POST: /Admin/Patients/DeleteConfirmed/5
        [HttpPost("DeleteConfirmed/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _patientService.Delete(id);
            TempData["Success"] = "✅ Patient deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ✅ GET: /Admin/Patients/Details/5
        [HttpGet("Details/{id}")]
        public IActionResult Details(int id)
        {
            var patient = _patientService.GetById(id);
            if (patient == null)
                return NotFound();

            var dto = new PatientDTO
            {
                PatientId = patient.PatientId,
                UserId = patient.UserId,
                UserFullName = patient.User?.FullName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Address = patient.Address,
                MedicalHistory = patient.MedicalHistory
            };

            return View("~/Views/Admin/Patients/Details.cshtml", dto);
        }
    }
}
