using BLL.Services;
using BLL.Services.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalRazor.Pages.Doctors.Records
{
    [Authorize(Roles = "DOCTOR")]
    public class EditModel : PageModel
    {
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly IPatientService _patientService;

        public EditModel(IMedicalRecordService medicalRecordService, IPatientService patientService)
        {
            _medicalRecordService = medicalRecordService;
            _patientService = patientService;
        }

        [BindProperty]
        public MedicalRecord MedicalRecord { get; set; } = new();

        [BindProperty]
        public Patient Patient { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var record = await _medicalRecordService.GetByIdAsync(id);
            if (record == null) return NotFound();

            MedicalRecord = record;
            Patient = record.Patient;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            ModelState.Remove("MedicalRecord.Doctor");
            ModelState.Remove("MedicalRecord.Patient");
            ModelState.Remove("Patient.User");

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    Console.WriteLine($"Validation error: {error.ErrorMessage}");
                return Page();
            }


            var record = await _medicalRecordService.GetByIdAsync(id);
            if (record == null) return NotFound();

            //  Cập nhật Medical Record
            record.Diagnosis = MedicalRecord.Diagnosis;
            record.Prescription = MedicalRecord.Prescription;
            record.CreatedAt = DateTime.Now;

            await _medicalRecordService.UpdateAsync(record);

            // Cập nhật thông tin bệnh nhân
            var patient = await _patientService.GetByIdAsync(record.PatientId);
            if (patient != null)
            {
                patient.PatientName = Patient.PatientName;
                patient.Phone = Patient.Phone;
                patient.Address = Patient.Address;
                patient.Gender = Patient.Gender;
                patient.DateOfBirth = Patient.DateOfBirth;
                patient.MedicalHistory = Patient.MedicalHistory;

                await _patientService.UpdateAsync(patient);
            }

            TempData["Success"] = "Record and patient info updated successfully!";
            return RedirectToPage("/Doctors/Records/Details", new { id });
        }
    }
}
