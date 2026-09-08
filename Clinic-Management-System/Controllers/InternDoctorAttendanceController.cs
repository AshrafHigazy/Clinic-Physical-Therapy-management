using Clinic_Management_System.Models;
using Clinic_Management_System.Services.InternDoctorAttendances;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]
    public class InternDoctorAttendanceController : Controller
    {
        private readonly IInternDoctorAttendanceService _attendanceService;

        public InternDoctorAttendanceController(IInternDoctorAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        #region Index - عرض حضور اليوم أو تاريخ معين
        public IActionResult Index(DateTime? date)
        {
            var selectedDate = date ?? DateTime.Today;
            var attendances = _attendanceService.GetAttendancesByDate(selectedDate);

            ViewBag.SelectedDate = selectedDate;
            return View(attendances);
        }
        #endregion

        #region Create - إضافة Attendance من صفحة الدكتور أو مستقلة
        [HttpGet]
        public IActionResult Create(int? doctorId)
        {
            var activeDoctors = _attendanceService.GetActiveInternDoctors();

            ViewBag.Doctors = activeDoctors;
            ViewBag.SelectedDoctorId = doctorId;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(InternDoctorAttendance attendance)
        {
            if (!ModelState.IsValid)
                return View(attendance);

            var result = _attendanceService.RecordAttendance(attendance);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message!);
                ViewBag.Doctors = _attendanceService.GetActiveInternDoctors();
                ViewBag.SelectedDoctorId = attendance.InternDoctorId;
                return View(attendance);
            }

            return RedirectToAction("GetById", "InternDoctors", new { id = attendance.InternDoctorId });
        }
        #endregion

        #region GetByDoctor - عرض كل الحضور لدكتور معين
        public IActionResult GetByDoctor(int id)
        {
            var doctor = _attendanceService.GetDoctorWithAttendances(id);
            if (doctor == null)
                return NotFound();

            return View(doctor);
        }
        #endregion

        #region Dashboard - تقرير شامل عن حضور دكتور واحد
        public IActionResult Dashboard(int id, int? month = null, int? year = null, DateTime? from = null, DateTime? to = null)
        {
            var report = _attendanceService.GetDoctorAttendanceReport(id, from, to);
            if (report == null)
                return NotFound();

            ViewBag.TotalDays = report.TotalDays;
            ViewBag.TotalHours = report.TotalHours;
            ViewBag.AvgHours = report.AvgHours;
            ViewBag.DoctorName = report.DoctorName;
            ViewBag.StartDate = report.StartDate.ToShortDateString();
            ViewBag.EndDate = report.EndDate.ToShortDateString();

            return View(report.Attendances);
        }
        #endregion

        [HttpPost]
        public IActionResult QuickCheckIn(int doctorId)
        {
            var result = _attendanceService.QuickCheckIn(doctorId);
            if (result.Message == "Doctor not found or inactive.")
                return NotFound(result.Message);

            TempData["Message"] = result.Message;
            return RedirectToAction("Index", "InternDoctors");
        }

        [HttpPost]
        public IActionResult QuickCheckOut(int doctorId)
        {
            var result = _attendanceService.QuickCheckOut(doctorId);
            if (result.Message == "Doctor not found or inactive.")
                return NotFound(result.Message);

            TempData["Message"] = result.Message;
            return RedirectToAction("Index", "InternDoctors");
        }
    }
}