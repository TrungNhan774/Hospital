using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HospitalRazor.Pages.Doctors.Records
{
    [Authorize(Roles = "DOCTOR")]
    public class IndexModel : PageModel
    {
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly IDoctorRepository _doctorRepository;

        public IndexModel(IMedicalRecordService medicalRecordService,
                          IDoctorRepository doctorRepository)
        {
            _medicalRecordService = medicalRecordService;
            _doctorRepository = doctorRepository;
        }

        public IEnumerable<MedicalRecord> Records { get; set; } = new List<MedicalRecord>();
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public int DoctorId { get; set; }
        public int UserId { get; set; }

        public async Task OnGetAsync()
        {
            // Lấy user id từ claim
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return;

            UserId = userId;

            var doctor = await _doctorRepository.GetByUserIdAsync(userId);
            if (doctor == null)
                return;

            DoctorId = doctor.DoctorId;

            // Lấy tất cả hồ sơ bệnh án
            var records = await _medicalRecordService.GetRecordsForDoctorAsync(doctor.DoctorId);

            // Nếu có từ khóa tìm kiếm thì lọc
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                SearchTerm = SearchTerm.Trim().ToLower();
                records = records.Where(r =>
                    (r.Patient?.PatientName != null && r.Patient.PatientName.ToLower().Contains(SearchTerm)) ||
                    (r.Patient?.Phone != null && r.Patient.Phone.Contains(SearchTerm))
                );
            }

            Records = records;
        }
    }
}
