using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]

    public class InternDoctorAttendanceController : Controller
    {
        private readonly IInternDoctorAttendanceRepository _internDoctorAttendanceRepository;

        public InternDoctorAttendanceController(IInternDoctorAttendanceRepository internDoctorAttendanceRepository)
        {
            _internDoctorAttendanceRepository = internDoctorAttendanceRepository;
        }

        #region Index - عرض حضور اليوم أو تاريخ معين
        public IActionResult Index(DateTime? date)
        {
            var selectedDate = date ?? DateTime.Today;

            var attendances = _internDoctorAttendanceRepository.GetAttendancesByDate(selectedDate);

            ViewBag.SelectedDate = selectedDate;
            return View(attendances);
        }
        #endregion

        #region Create - إضافة Attendance من صفحة الدكتور أو مستقلة
        [HttpGet]
        public IActionResult Create(int? doctorId)
        {
            var activeDoctors = _internDoctorAttendanceRepository.GetActiveInternDoctors();

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

            var doctor = _internDoctorAttendanceRepository.GetActiveInternDoctor(attendance.InternDoctorId);

            if (doctor == null)
            {
                ModelState.AddModelError("", "لا يمكن تسجيل الحضور إلا للدكاترة الفعّالين.");
                return View(attendance);
            }

            bool exists = _internDoctorAttendanceRepository.AttendanceExists(attendance.InternDoctorId, attendance.Date);

            if (exists)
            {
                ModelState.AddModelError("", "تم تسجيل الحضور لهذا اليوم بالفعل.");
                return View(attendance);
            }

            _internDoctorAttendanceRepository.AddAttendance(attendance);

            return RedirectToAction("GetById", "InternDoctors", new { id = attendance.InternDoctorId });

        }
        #endregion

        #region GetByDoctor - عرض كل الحضور لدكتور معين
        public IActionResult GetByDoctor(int id)
        {
            var doctor = _internDoctorAttendanceRepository.GetInternDoctorWithAttendances(id);

            if (doctor == null)
                return NotFound();

            return View(doctor);
        }
        #endregion

        #region Dashboard - تقرير شامل عن حضور دكتور واحد
        public IActionResult Dashboard(int id, int? month = null, int? year = null, DateTime? from = null, DateTime? to = null)
        {
            var doctor = _internDoctorAttendanceRepository.GetInternDoctorWithAttendances(id);

            if (doctor == null)
                return NotFound();

            DateTime startDate;
            DateTime endDate;

            if (from.HasValue && to.HasValue)
            {
                startDate = from.Value.Date;
                endDate = to.Value.Date;
            }

            else
            {
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
            }

            var filteredAttendances = doctor.Attendances
                .Where(a => a.Date.Date >= startDate && a.Date.Date <= endDate)
                .OrderBy(a => a.Date)
                .ToList();

            var totalDays = filteredAttendances.Count;
            var totalHours = Math.Round(filteredAttendances.Sum(a => a.Hours ?? 0), 2);
            var avgHours = totalDays > 0 ? Math.Round(totalHours / totalDays, 2) : 0;

            ViewBag.TotalDays = totalDays;
            ViewBag.TotalHours = totalHours;
            ViewBag.AvgHours = avgHours;
            ViewBag.DoctorName = doctor.FullName;
            ViewBag.StartDate = startDate.ToShortDateString();
            ViewBag.EndDate = endDate.ToShortDateString();

            return View(filteredAttendances);
        }

        #endregion

        [HttpPost]
        public IActionResult QuickCheckIn(int doctorId)
        {
            var doctor = _internDoctorAttendanceRepository.GetActiveInternDoctor(doctorId);
            if (doctor == null)
                return NotFound("Doctor not found or inactive.");

            bool alreadyCheckedIn = _internDoctorAttendanceRepository.AttendanceExists(doctorId, DateTime.Today);

            if (alreadyCheckedIn)
            {
                TempData["Message"] = "✅ تم تسجيل الحضور بالفعل اليوم.";
                return RedirectToAction("Index", "InternDoctors");
            }

            var attendance = new InternDoctorAttendance
            {
                InternDoctorId = doctorId,
                Date = DateTime.Today,
                CheckIn = DateTime.Now,
                CheckOut = null,
                Hours = null
            };

            _internDoctorAttendanceRepository.AddAttendance(attendance);

            TempData["Message"] = $"✅ تم تسجيل حضور {doctor.FullName} في {DateTime.Now:HH:mm}";
            return RedirectToAction("Index", "InternDoctors");
        }

        [HttpPost]
        public IActionResult QuickCheckOut(int doctorId)
        {
            var doctor = _internDoctorAttendanceRepository.GetActiveInternDoctor(doctorId);
            if (doctor == null)
                return NotFound("Doctor not found or inactive.");

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var todayAttendance = _internDoctorAttendanceRepository.GetTodayAttendance(doctorId, today, tomorrow);

            if (todayAttendance == null)
            {
                TempData["Message"] = "⚠️ لم يتم تسجيل حضور هذا الطبيب اليوم.";
                return RedirectToAction("Index", "InternDoctors");
            }

            if (todayAttendance.CheckOut != null)
            {
                TempData["Message"] = "✅ تم تسجيل الانصراف بالفعل.";
                return RedirectToAction("Index", "InternDoctors");
            }

            todayAttendance.CheckOut = DateTime.Now;
            if (todayAttendance.CheckIn != null)
            {
                var duration = (todayAttendance.CheckOut.Value - todayAttendance.CheckIn.Value).TotalHours;
                todayAttendance.Hours = Math.Round(duration, 2);
            }

            _internDoctorAttendanceRepository.SaveChanges();

            TempData["Message"] = $"👋 تم تسجيل انصراف {doctor.FullName} في {DateTime.Now:HH:mm}";
            return RedirectToAction("Index", "InternDoctors");
        }

    }
}