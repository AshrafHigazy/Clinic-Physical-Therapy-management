using Clinic_Management_System.Services.ReceptionistAttendances;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]
    public class ReceptionistAttendancesController : Controller
    {
        private readonly IReceptionistAttendanceService _attendanceService;

        public ReceptionistAttendancesController(IReceptionistAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        public IActionResult StartShift()
        {
            var (receptionists, currentShifts) = _attendanceService.GetStartShiftData();

            ViewData["ReceptionistId"] = new SelectList(receptionists, "Id", "FullName");
            ViewData["CurrentShifts"] = currentShifts;

            ViewBag.Message = TempData["Message"];
            ViewBag.MessageType = TempData["MessageType"];

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartShift(int receptionistId)
        {
            if (receptionistId == 0)
                return BadRequest();

            var result = await _attendanceService.StartShiftAsync(receptionistId);

            TempData["Message"] = result.Message;
            TempData["MessageType"] = result.Success
                ? "success"
                : (result.Message != null && result.Message.Contains("أنهى عمله") ? "info" : "danger");

            return RedirectToAction(nameof(StartShift));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EndShift(int receptionistId)
        {
            var result = await _attendanceService.EndShiftAsync(receptionistId);

            TempData["Message"] = result.Message;
            TempData["MessageType"] = result.Success
                ? "success"
                : (result.Message != null && result.Message.Contains("أنهى عمله") ? "info" : "warning");

            return RedirectToAction(nameof(StartShift));
        }

        public async Task<IActionResult> AttendanceHistory(int? receptionistId)
        {
            if (receptionistId == null)
                return BadRequest();

            var history = await _attendanceService.GetAttendanceHistoryAsync(receptionistId);
            return View("AttendanceHistory", history);
        }

        public async Task<IActionResult> Index()
        {
            var attendances = await _attendanceService.GetAllAttendancesAsync();
            return View(attendances);
        }
    }
}