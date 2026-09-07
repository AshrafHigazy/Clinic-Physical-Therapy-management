using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]

    public class ReceptionistAttendancesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReceptionistAttendancesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult StartShift()
        {
            ViewData["ReceptionistId"] = new SelectList(_unitOfWork.ReceptionistAttendances.GetReceptionistsForSelect(), "Id", "FullName");

            var currentShifts = _unitOfWork.ReceptionistAttendances.GetCurrentShifts();
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

            var existingShift = await _unitOfWork.ReceptionistAttendances.GetCurrentShiftByReceptionistAsync(receptionistId);
            if (existingShift != null)
            {
                TempData["Message"] = "❌ هذا الموظف بدأ عمله بالفعل.";
                TempData["MessageType"] = "danger";
                return RedirectToAction(nameof(StartShift));
            }

            var today = DateTime.Today;
            var attendanceToday = await _unitOfWork.ReceptionistAttendances.GetAttendanceByReceptionistAndDateAsync(receptionistId, today);

            if (attendanceToday != null)
            {
                TempData["Message"] = "✅ هذا الموظف أنهى عمله اليوم.";
                TempData["MessageType"] = "info";
                return RedirectToAction(nameof(StartShift));
            }

            var shift = new ReceptionistCurrentShift
            {
                ReceptionistId = receptionistId,
                StartTime = DateTime.Now
            };

            _unitOfWork.ReceptionistAttendances.AddCurrentShift(shift);
            await _unitOfWork.SaveChangesAsync();

            TempData["Message"] = "✅ تم تسجيل بداية العمل بنجاح.";
            TempData["MessageType"] = "success";
            return RedirectToAction(nameof(StartShift));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EndShift(int receptionistId)
        {
            var shift = await _unitOfWork.ReceptionistAttendances.GetCurrentShiftWithReceptionistAsync(receptionistId);

            if (shift == null)
            {
                TempData["Message"] = "❌ هذا الموظف لم يبدأ عمله بعد.";
                TempData["MessageType"] = "warning";
                return RedirectToAction(nameof(StartShift));
            }

            var today = DateTime.Today;
            var alreadyEnded = await _unitOfWork.ReceptionistAttendances.AttendanceAlreadyEndedAsync(receptionistId, today);

            if (alreadyEnded)
            {
                TempData["Message"] = "✅ هذا الموظف أنهى عمله اليوم بالفعل.";
                TempData["MessageType"] = "info";
                return RedirectToAction(nameof(StartShift));
            }

            var attendance = new ReceptionistAttendance
            {
                ReceptionistId = shift.ReceptionistId,
                Date = today,
                CheckIn = shift.StartTime,
                CheckOut = DateTime.Now,
                Hours = (int)(DateTime.Now - shift.StartTime).TotalHours
            };

            _unitOfWork.ReceptionistAttendances.AddAttendanceAndRemoveShift(attendance, shift);
            await _unitOfWork.SaveChangesAsync();

            TempData["Message"] = "✅ تم تسجيل إنهاء العمل بنجاح.";
            TempData["MessageType"] = "success";
            return RedirectToAction(nameof(StartShift));
        }

        public async Task<IActionResult> AttendanceHistory(int? receptionistId)
        {
            if (receptionistId == null)
                return BadRequest();

            var history = await _unitOfWork.ReceptionistAttendances.GetAttendanceHistoryAsync(receptionistId);

            return View("AttendanceHistory", history);
        }

        public async Task<IActionResult> Index()
        {
            var attendances = await _unitOfWork.ReceptionistAttendances.GetAllAttendancesAsync();

            return View(attendances);
        }
    }
}