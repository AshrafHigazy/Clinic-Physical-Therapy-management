using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly int OpeningHour = 12;
        private readonly int LastStartHour = 22;
        private readonly int MaxPerHour = 4;
        private readonly TimeSpan AppointmentDuration = TimeSpan.FromHours(1);

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Calendar()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAppointments()
        {
            var appointments = await _context.Appointment
                .Include(a => a.Patient)
                .Include(a => a.Receptionist)
                .Select(a => new
                {
                    id = a.Id,
                    title = a.Patient.FullName,
                    start = a.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = a.EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    isAttended = a.IsAttended,
                    isCanceled = a.IsCanceled,
                    receptionist = a.Receptionist != null ? (a.Receptionist.FullName ?? a.Receptionist.Id.ToString()) : "غير محدد",
                    description = a.IsCanceled ? " غاب" : (a.IsAttended ? " حضر" : " لم يتأكد"),

                    color = a.IsCanceled ? "#dc3545" : "#28a745"
                })
                .ToListAsync();

            return Json(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmBooking(int patientId, DateTime date)
        {

            var receptionists = await _context.Receptionist
                .OrderBy(r => r.FullName)
                .ToListAsync();

            ViewBag.Receptionists = receptionists;
            ViewBag.PatientId = patientId;
            ViewBag.Date = date.ToString("yyyy-MM-ddTHH:mm");

            return PartialView("_ConfirmBooking", model: null);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int patientId, string start, int receptionistId)
        {
            if (!DateTime.TryParseExact(
                start,
                "yyyy-MM-ddTHH:mm",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime startTime))
            {
                return Json(new { success = false, message = "❌ صيغة التاريخ غير صحيحة." });
            }

            var receptionist = await _context.Receptionist.FindAsync(receptionistId);
            if (receptionist == null)
            {
                return Json(new { success = false, message = "❌ اختيار سكرتير غير صالح." });
            }

            startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, startTime.Minute, 0);
            DateTime endTime = startTime.Add(AppointmentDuration);

            if (startTime.Hour < OpeningHour || startTime.Hour > LastStartHour)
            {
                return Json(new { success = false, message = $"⚠ ساعات العمل من {OpeningHour}:00 إلى {LastStartHour + 1}:00" });
            }

            bool patientOverlap = await _context.Appointment.AnyAsync(a =>
                a.PatientId == patientId &&
                a.StartTime < endTime &&
                a.EndTime > startTime &&
                !a.IsCanceled
            );

            if (patientOverlap)
            {
                return Json(new { success = false, message = "⚠ المريض لديه موعد متداخل في نفس الفترة." });
            }

            int overlappingCount = await _context.Appointment.CountAsync(a =>
                a.StartTime < endTime &&
                a.EndTime > startTime &&
                !a.IsCanceled
            );

            if (overlappingCount >= MaxPerHour)
            {
                return Json(new { success = false, message = "⚠ هذا التوقيت ممتلئ بالفعل (4 مرضى كحد أقصى)." });
            }

            var appointment = new Appointment
            {
                PatientId = patientId,
                ReceptionistId = receptionistId,
                StartTime = startTime,
                EndTime = endTime,
                IsWithMainDoctor = true
            };

            _context.Appointment.Add(appointment);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "✅ تم حجز الموعد بنجاح." });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var appt = await _context.Appointment
                .Include(a => a.Patient)
                .Include(a => a.Receptionist)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appt == null) return NotFound();

            return PartialView("_AppointmentDetails", appt);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var appt = await _context.Appointment.FindAsync(id);
            if (appt == null) return NotFound();

            appt.IsCanceled = true;
            appt.IsAttended = false;
            _context.Appointment.Update(appt);
            await _context.SaveChangesAsync();
            Console.WriteLine("✅ SaveChangesAsync executed");

            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, bool isAttended, bool isCanceled)
        {
            var appt = await _context.Appointment.FindAsync(id);
            if (appt == null) return NotFound();

            appt.IsAttended = isAttended;
            appt.IsCanceled = isCanceled;
            _context.Appointment.Update(appt);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var appt = await _context.Appointment.FindAsync(id);
            if (appt == null) return NotFound();

            _context.Appointment.Remove(appt);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> DetailsPage(int id)
        {
            var appointment = await _context.Appointment
                .Include(a => a.Patient)
                .Include(a => a.Receptionist)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            return View("DetailsPage", appointment);
        }

    }
}
