using Clinic_Management_System.Services.Appointments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        public IActionResult Calendar()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAppointments()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            return Json(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmBooking(int patientId, DateTime date)
        {
            var (receptionists, formattedDate) = await _appointmentService.GetConfirmBookingDataAsync(patientId, date);

            ViewBag.Receptionists = receptionists;
            ViewBag.PatientId = patientId;
            ViewBag.Date = formattedDate;

            return PartialView("_ConfirmBooking", model: null);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int patientId, string start, int receptionistId)
        {
            var result = await _appointmentService.CreateAppointmentAsync(patientId, start, receptionistId);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var appt = await _appointmentService.GetAppointmentDetailsAsync(id);
            if (appt == null) return NotFound();

            return PartialView("_AppointmentDetails", appt);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _appointmentService.CancelAppointmentAsync(id);
            if (!result.Success) return NotFound();

            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, bool isAttended, bool isCanceled)
        {
            var result = await _appointmentService.UpdateAppointmentStatusAsync(id, isAttended, isCanceled);
            if (!result.Success) return NotFound();

            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _appointmentService.DeleteAppointmentAsync(id);
            if (!result.Success) return NotFound();

            return Ok(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> DetailsPage(int id)
        {
            var appointment = await _appointmentService.GetAppointmentDetailsAsync(id);
            if (appointment == null)
                return NotFound();

            return View("DetailsPage", appointment);
        }
    }
}