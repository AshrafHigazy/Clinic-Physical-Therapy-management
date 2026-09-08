using Clinic_Management_System.Models;
using Clinic_Management_System.Models.ViewModels;
using Clinic_Management_System.Repositories;
using Clinic_Management_System.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Clinic_Management_System.Services.InternDoctorAttendances
{
    public class InternDoctorAttendanceService : IInternDoctorAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InternDoctorAttendanceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<InternDoctorAttendance> GetAttendancesByDate(DateTime selectedDate)
        {
            return _unitOfWork.InternDoctorAttendances.GetAttendancesByDate(selectedDate);
        }

        public List<InternDoctor> GetActiveInternDoctors()
        {
            return _unitOfWork.InternDoctorAttendances.GetActiveInternDoctors();
        }

        public ServiceResult RecordAttendance(InternDoctorAttendance attendance)
        {
            var doctor = _unitOfWork.InternDoctorAttendances.GetActiveInternDoctor(attendance.InternDoctorId);
            if (doctor == null)
            {
                return ServiceResult.Fail("لا يمكن تسجيل الحضور إلا للدكاترة الفعّالين.");
            }

            bool exists = _unitOfWork.InternDoctorAttendances.AttendanceExists(attendance.InternDoctorId, attendance.Date);
            if (exists)
            {
                return ServiceResult.Fail("تم تسجيل الحضور لهذا اليوم بالفعل.");
            }

            _unitOfWork.InternDoctorAttendances.AddAttendance(attendance);
            _unitOfWork.SaveChanges();

            return ServiceResult.Ok();
        }

        public InternDoctor? GetDoctorWithAttendances(int id)
        {
            return _unitOfWork.InternDoctorAttendances.GetInternDoctorWithAttendances(id);
        }

        public DoctorAttendanceReport? GetDoctorAttendanceReport(int id, DateTime? from, DateTime? to)
        {
            var doctor = _unitOfWork.InternDoctorAttendances.GetInternDoctorWithAttendances(id);
            if (doctor == null) return null;

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

            var filteredAttendances = (doctor.Attendances ?? new List<InternDoctorAttendance>())
                .Where(a => a.Date.Date >= startDate && a.Date.Date <= endDate)
                .OrderBy(a => a.Date)
                .ToList();

            var totalDays = filteredAttendances.Count;
            var totalHours = Math.Round(filteredAttendances.Sum(a => a.Hours ?? 0), 2);
            var avgHours = totalDays > 0 ? Math.Round(totalHours / totalDays, 2) : 0;

            return new DoctorAttendanceReport
            {
                DoctorName = doctor.FullName,
                StartDate = startDate,
                EndDate = endDate,
                TotalDays = totalDays,
                TotalHours = totalHours,
                AvgHours = avgHours,
                Attendances = filteredAttendances
            };
        }

        public ServiceResult QuickCheckIn(int doctorId)
        {
            var doctor = _unitOfWork.InternDoctorAttendances.GetActiveInternDoctor(doctorId);
            if (doctor == null)
            {
                return ServiceResult.Fail("Doctor not found or inactive.");
            }

            bool alreadyCheckedIn = _unitOfWork.InternDoctorAttendances.AttendanceExists(doctorId, DateTime.Today);
            if (alreadyCheckedIn)
            {
                return ServiceResult.Fail("✅ تم تسجيل الحضور بالفعل اليوم.");
            }

            var attendance = new InternDoctorAttendance
            {
                InternDoctorId = doctorId,
                Date = DateTime.Today,
                CheckIn = DateTime.Now,
                CheckOut = null,
                Hours = null
            };

            _unitOfWork.InternDoctorAttendances.AddAttendance(attendance);
            _unitOfWork.SaveChanges();

            return ServiceResult.Ok($"✅ تم تسجيل حضور {doctor.FullName} في {DateTime.Now:HH:mm}");
        }

        public ServiceResult QuickCheckOut(int doctorId)
        {
            var doctor = _unitOfWork.InternDoctorAttendances.GetActiveInternDoctor(doctorId);
            if (doctor == null)
            {
                return ServiceResult.Fail("Doctor not found or inactive.");
            }

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var todayAttendance = _unitOfWork.InternDoctorAttendances.GetTodayAttendance(doctorId, today, tomorrow);
            if (todayAttendance == null)
            {
                return ServiceResult.Fail("⚠️ لم يتم تسجيل حضور هذا الطبيب اليوم.");
            }

            if (todayAttendance.CheckOut != null)
            {
                return ServiceResult.Fail("✅ تم تسجيل الانصراف بالفعل.");
            }

            todayAttendance.CheckOut = DateTime.Now;
            if (todayAttendance.CheckIn != null)
            {
                var duration = (todayAttendance.CheckOut.Value - todayAttendance.CheckIn.Value).TotalHours;
                todayAttendance.Hours = Math.Round(duration, 2);
            }

            _unitOfWork.SaveChanges();

            return ServiceResult.Ok($"👋 تم تسجيل انصراف {doctor.FullName} في {DateTime.Now:HH:mm}");
        }
    }
}
