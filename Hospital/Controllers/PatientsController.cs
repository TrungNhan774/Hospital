using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize(Roles = "CUSTOMER")]
public class PatientsController : Controller
{
    private readonly IMedicalRecordService _medicalRecordService;

    public PatientsController(IMedicalRecordService medicalRecordService)
    {
        _medicalRecordService = medicalRecordService;
    }

    public IActionResult Index()
    {
        return RedirectToAction("MyRecords");
    }

    [HttpGet("MyRecords")]
    public async Task<IActionResult> MyRecords()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized("Không tìm thấy thông tin người dùng đang đăng nhập.");
        }

        var records = await _medicalRecordService.GetRecordsByUserIdAsync(userId);

        return View(records);
    }

    [HttpGet("RecordDetails/{id}")]
    public async Task<IActionResult> RecordDetails(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized("Không tìm thấy thông tin người dùng đang đăng nhập.");
        }

        var detailDto = await _medicalRecordService.GetRecordDetailAsync(id, userId);

        if (detailDto == null)
        {
            return NotFound("Không tìm thấy hồ sơ hoặc bạn không có quyền truy cập.");
        }

        return View(detailDto);
    }
}
