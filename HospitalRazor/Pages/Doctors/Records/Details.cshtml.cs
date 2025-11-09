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
    public class DetailsModel : PageModel
    {
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly IDoctorService _doctorService;

        public DetailsModel(IMedicalRecordService medicalRecordService, IDoctorService doctorService)
        {
            _medicalRecordService = medicalRecordService;
            _doctorService = doctorService;
        }

        [BindProperty]
        public MedicalRecord? Record { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Forbid();

            var doctor = await _doctorService.GetDByUserIdAsync(userId);
            if (doctor == null)
                return Forbid();

            Record = await _medicalRecordService.GetRecordWithMedicinesForDoctorAsync(id, doctor.DoctorId);
            if (Record == null)
                return NotFound(); // hoặc Forbid nếu muốn che giấu existence

            return Page();
        }
    }
}
