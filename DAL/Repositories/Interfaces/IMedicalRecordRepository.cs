using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IMedicalRecordRepository
    {
        Task<IEnumerable<MedicalRecord>> GetByDoctorAsync(int doctorId);
        Task<MedicalRecord?> GetByIdAndDoctorAsync(int recordId, int doctorId);
    }
}
