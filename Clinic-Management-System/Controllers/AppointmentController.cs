using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepository;

        private readonly int OpeningHour = 12;
        private readonly int LastStartHour = 22;
        private readonly int MaxPerHour = 4;
        private readonly TimeSpan AppointmentDuration = TimeSpan.FromHours(1);

        public AppointmentController(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public IActionResult Calendar()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAppointments()
        {
            var appointments = await _appointmentRepository.GetAllAppointmentsAsync();

            return Json(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmBooking(int patientId, DateTime date)
        {

            var receptionists = await _appointmentRepository.GetReceptionistsAsync();

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

            var receptionist = await _appointmentRepository.FindReceptionistAsync(receptionistId);
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

            bool patientOverlap = await _appointmentRepository.PatientHasOverlappingAppointmentAsync(patientId, startTime, endTime);

            if (patientOverlap)
            {
                return Json(new { success = false, message = "⚠ المريض لديه موعد متداخل في نفس الفترة." });
            }

            int overlappingCount = await _appointmentRepository.CountOverlappingAppointmentsAsync(startTime, endTime);

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

            await _appointmentRepository.AddAppointmentAsync(appointment);

            return Json(new { success = true, message = "✅ تم حجز الموعد بنجاح." });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var appt = await _appointmentRepository.GetAppointmentDetailsAsync(id);

            if (appt == null) return NotFound();

            return PartialView("_AppointmentDetails", appt);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var appt = await _appointmentRepository.FindAppointmentAsync(id);
            if (appt == null) return NotFound();

            appt.IsCanceled = true;
            appt.IsAttended = false;
            await _appointmentRepository.UpdateAppointmentAsync(appt);
            Console.WriteLine("✅ SaveChangesAsync executed");

            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, bool isAttended, bool isCanceled)
        {
            var appt = await _appointmentRepository.FindAppointmentAsync(id);
            if (appt == null) return NotFound();

            appt.IsAttended = isAttended;
            appt.IsCanceled = isCanceled;
            await _appointmentRepository.UpdateAppointmentAsync(appt);

            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var appt = await _appointmentRepository.FindAppointmentAsync(id);
            if (appt == null) return NotFound();

            await _appointmentRepository.RemoveAppointmentAsync(appt);

            return Ok(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> DetailsPage(int id)
        {
            var appointment = await _appointmentRepository.GetAppointmentDetailsAsync(id);

            if (appointment == null)
                return NotFound();

            return View("DetailsPage", appointment);
        }

    }
}