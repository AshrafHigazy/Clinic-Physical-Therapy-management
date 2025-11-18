using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]

    public class ReceptionistAttendancesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReceptionistAttendancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult StartShift()
        {
            ViewData["ReceptionistId"] = new SelectList(_context.Receptionist, "Id", "FullName");

            var currentShifts = _context.ReceptionistCurrentShifts
                .Include(c => c.Receptionist)
                .ToList();
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

            // تحقق هل الموظف بدأ عمله بالفعل ولم ينتهِ
            var existingShift = await _context.ReceptionistCurrentShifts
                .FirstOrDefaultAsync(c => c.ReceptionistId == receptionistId);
            if (existingShift != null)
            {
                TempData["Message"] = "❌ هذا الموظف بدأ عمله بالفعل.";
                TempData["MessageType"] = "danger";
                return RedirectToAction(nameof(StartShift));
            }

            // تحقق هل الموظف أنهى عمله اليوم بالفعل
            var today = DateTime.Today;
            var attendanceToday = await _context.ReceptionistAttendance
                .FirstOrDefaultAsync(a => a.ReceptionistId == receptionistId && a.Date == today);

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

            _context.ReceptionistCurrentShifts.Add(shift);
            await _context.SaveChangesAsync();

            TempData["Message"] = "✅ تم تسجيل بداية العمل بنجاح.";
            TempData["MessageType"] = "success";
            return RedirectToAction(nameof(StartShift));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EndShift(int receptionistId)
        {
            var shift = await _context.ReceptionistCurrentShifts
                .Include(c => c.Receptionist)
                .FirstOrDefaultAsync(c => c.ReceptionistId == receptionistId);

            if (shift == null)
            {
                TempData["Message"] = "❌ هذا الموظف لم يبدأ عمله بعد.";
                TempData["MessageType"] = "warning";
                return RedirectToAction(nameof(StartShift));
            }

            // تحقق لو أنهى العمل اليوم بالفعل
            var today = DateTime.Today;
            var alreadyEnded = await _context.ReceptionistAttendance
                .AnyAsync(a => a.ReceptionistId == receptionistId && a.Date == today);

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

            _context.ReceptionistAttendance.Add(attendance);
            _context.ReceptionistCurrentShifts.Remove(shift);

            await _context.SaveChangesAsync();

            TempData["Message"] = "✅ تم تسجيل إنهاء العمل بنجاح.";
            TempData["MessageType"] = "success";
            return RedirectToAction(nameof(StartShift));
        }

        public async Task<IActionResult> AttendanceHistory(int? receptionistId)
        {
            if (receptionistId == null)
                return BadRequest();

            var history = await _context.ReceptionistAttendance
                .Where(a => a.ReceptionistId == receptionistId)
                .Include(a => a.Receptionist)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View("AttendanceHistory", history);
        }

        public async Task<IActionResult> Index()
        {
            var attendances = _context.ReceptionistAttendance
                .Include(r => r.Receptionist)
                .OrderByDescending(a => a.Date);

            return View(await attendances.ToListAsync());
        }
    }
}
