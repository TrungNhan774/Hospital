using BLL.Services.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalRazor.Pages.Doctors
{
    [Authorize(Roles = "DOCTOR")]
    public class PrescribeModel : PageModel
    {
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly IMedicineService _medicineService;

        public PrescribeModel(IMedicalRecordService medicalRecordService, IMedicineService medicineService)
        {
            _medicalRecordService = medicalRecordService;
            _medicineService = medicineService;
        }

        [BindProperty]
        public MedicalRecord? MedicalRecord { get; set; }

        [BindProperty]
        public Patient? Patient { get; set; }

        [BindProperty]
        public List<PrescriptionItem> PrescriptionItems { get; set; } = new();

        public List<Medicine> Medicines { get; set; } = new();

        public class PrescriptionItem
        {
            public int MedicineId { get; set; }
            public string? Dosage { get; set; }
            public int? Quantity { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int recordId)
        {
            MedicalRecord = await _medicalRecordService.GetByIdAsync(recordId);
            if (MedicalRecord == null)
                return NotFound();

            Patient = MedicalRecord.Patient;
            Medicines = (await _medicineService.GetAllAsync()).ToList();

            // Load danh sách thuốc đã kê (nếu có)
            if (MedicalRecord.MedicalRecordMedicines != null && MedicalRecord.MedicalRecordMedicines.Any())
            {
                PrescriptionItems = MedicalRecord.MedicalRecordMedicines.Select(m => new PrescriptionItem
                {
                    MedicineId = m.MedicineId,
                    Dosage = m.Dosage,
                    Quantity = m.Quantity
                }).ToList();
            }
            else
            {
                // Nếu chưa có thuốc nào -> thêm 1 dòng trống để nhập
                PrescriptionItems.Add(new PrescriptionItem());
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync(int recordId)
        {
            var record = await _medicalRecordService.GetByIdAsync(recordId);
            if (record == null)
                return NotFound();
            // Cập nhật Diagnosis và Prescription từ form
            record.Diagnosis = MedicalRecord?.Diagnosis;
            record.Prescription = MedicalRecord?.Prescription;

            // Đảm bảo đã load danh sách thuốc cũ
            var existingMedicines = record.MedicalRecordMedicines.ToList();

            foreach (var item in PrescriptionItems)
            {
                if (item.MedicineId <= 0)
                    continue;

                var existing = existingMedicines.FirstOrDefault(m => m.MedicineId == item.MedicineId);

                if (existing != null)
                {
                    // Cập nhật thuốc đã có
                    existing.Dosage = item.Dosage;
                    existing.Quantity = item.Quantity;
                }
                else
                {
                    // Thêm thuốc mới
                    record.MedicalRecordMedicines.Add(new MedicalRecordMedicine
                    {
                        RecordId = recordId,
                        MedicineId = item.MedicineId,
                        Dosage = item.Dosage,
                        Quantity = item.Quantity
                    });
                }
            }

            await _medicalRecordService.UpdateAsync(record);
            TempData["Success"] = "Prescription saved successfully!";
            return RedirectToPage("/Doctors/Records/Details", new { id = recordId });
        }


    }
}
