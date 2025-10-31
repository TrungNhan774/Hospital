using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<(bool Success, string Message)> BookAppointmentAsync(
            int scheduleId, int doctorId, int departmentId, int patientId, int serviceWeight);
    }
}
