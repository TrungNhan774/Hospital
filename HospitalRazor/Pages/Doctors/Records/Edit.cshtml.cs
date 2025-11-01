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
        private readonly IMedicineService _medicineService;

        public EditModel(
            IMedicalRecordService medicalRecordService,
            IPatientService patientService,
            IMedicineService medicineService)
        {
            _medicalRecordService = medicalRecordService;
            _patientService = patientService;
            _medicineService = medicineService;
        }

        [BindProperty]
        public MedicalRecord MedicalRecord { get; set; } = new();

        [BindProperty]
        public Patient Patient { get; set; } = new();

        [BindProperty]
        public List<PrescriptionItem> PrescriptionItems { get; set; } = new();

        public List<Medicine> Medicines { get; set; } = new();

        public class PrescriptionItem
        {
            public int MedicineId { get; set; }
            public string? Dosage { get; set; }
            public int? Quantity { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var record = await _medicalRecordService.GetByIdAsync(id);
            if (record == null)
                return NotFound();

            MedicalRecord = record;
            Patient = record.Patient;
            Medicines = (await _medicineService.GetAllAsync()).ToList();

            // Load danh sách thuốc (nếu có)
            if (record.MedicalRecordMedicines != null && record.MedicalRecordMedicines.Any())
            {
                PrescriptionItems = record.MedicalRecordMedicines.Select(m => new PrescriptionItem
                {
                    MedicineId = m.MedicineId,
                    Dosage = m.Dosage,
                    Quantity = m.Quantity
                }).ToList();
            }
            else
            {
                PrescriptionItems.Add(new PrescriptionItem());
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Bỏ qua các navigation không cần validate
            ModelState.Remove("MedicalRecord.Patient");
            ModelState.Remove("MedicalRecord.Doctor");
            ModelState.Remove("Patient.User");

            if (!ModelState.IsValid)
            {
                foreach (var e in ModelState.Values.SelectMany(v => v.Errors))
                    Console.WriteLine($"Validation error: {e.ErrorMessage}");
                Medicines = (await _medicineService.GetAllAsync()).ToList();
                return Page();
            }

            var record = await _medicalRecordService.GetByIdAsync(id);
            if (record == null)
                return NotFound();

            // Update thông tin hồ sơ
            record.Diagnosis = MedicalRecord.Diagnosis;
            record.Prescription = MedicalRecord.Prescription;
            record.CreatedAt = DateTime.Now;

            //  Update bệnh nhân
            var patient = await _patientService.GetByIdDAsync(record.PatientId);
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

            //  Medicines
            var existingMedicines = record.MedicalRecordMedicines.ToList();

            //  Xóa thuốc bị bỏ khỏi form
            var newMedicineIds = PrescriptionItems
                .Where(i => i.MedicineId > 0)
                .Select(i => i.MedicineId)
                .ToHashSet();

            var toRemove = existingMedicines
                .Where(m => !newMedicineIds.Contains(m.MedicineId))
                .ToList();

            foreach (var removeItem in toRemove)
            {
                record.MedicalRecordMedicines.Remove(removeItem);
            }

            //  Cập nhật hoặc thêm thuốc
            foreach (var item in PrescriptionItems)
            {
                if (item.MedicineId <= 0)
                    continue;

                var existing = existingMedicines.FirstOrDefault(m => m.MedicineId == item.MedicineId);
                if (existing != null)
                {
                    existing.Dosage = item.Dosage;
                    existing.Quantity = item.Quantity;
                }
                else
                {
                    record.MedicalRecordMedicines.Add(new MedicalRecordMedicine
                    {
                        RecordId = id,
                        MedicineId = item.MedicineId,
                        Dosage = item.Dosage,
                        Quantity = item.Quantity
                    });
                }
            }

            await _medicalRecordService.UpdateAsync(record);

            TempData["Success"] = "Record, patient, and medicines updated successfully!";
            return RedirectToPage("/Doctors/Records/Details", new { id });
        }
    }
}
