using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories;
using Clinic_Management_System.Services.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic_Management_System.Services.InternDoctors
{
    public class InternDoctorService : IInternDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InternDoctorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<InternDoctor>> GetDoctorsAsync(string? search)
        {
            return await _unitOfWork.InternDoctors.GetDoctorsAsync(search);
        }

        public async Task<ServiceResult> CreateDoctorAsync(InternDoctor doctor)
        {
            _unitOfWork.InternDoctors.AddDoctor(doctor);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult.Ok("تمت إضافة الطبيب بنجاح.");
        }

        public InternDoctor? GetDoctorWithAttendances(int id)
        {
            return _unitOfWork.InternDoctors.GetDoctorWithAttendances(id);
        }

        public async Task<InternDoctor?> FindDoctorAsync(int id)
        {
            return await _unitOfWork.InternDoctors.FindDoctorAsync(id);
        }

        public async Task<InternDoctor?> GetDoctorForDeleteAsync(int id)
        {
            return await _unitOfWork.InternDoctors.GetDoctorByFilterAsync(id);
        }

        public async Task<ServiceResult> UpdateDoctorAsync(int id, InternDoctor doctor)
        {
            if (id != doctor.InternDoctorId)
            {
                return ServiceResult.Fail("معرف الطبيب غير متطابق.");
            }

            _unitOfWork.InternDoctors.UpdateDoctor(doctor);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult.Ok("تم تعديل بيانات الطبيب بنجاح.");
        }

        public async Task<ServiceResult> DeleteDoctorAsync(int id)
        {
            var doctor = await _unitOfWork.InternDoctors.FindDoctorAsync(id);
            if (doctor == null)
            {
                return ServiceResult.Fail("الطبيب غير موجود.");
            }

            _unitOfWork.InternDoctors.RemoveDoctor(doctor);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult.Ok("تم حذف الطبيب بنجاح.");
        }

        public async Task<ServiceResult> ToggleActiveAsync(int id)
        {
            var doctor = await _unitOfWork.InternDoctors.FindDoctorAsync(id);
            if (doctor == null)
            {
                return ServiceResult.Fail("الطبيب غير موجود.");
            }

            doctor.IsActive = !doctor.IsActive;
            _unitOfWork.InternDoctors.UpdateDoctor(doctor);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult.Ok(doctor.IsActive ? "تم تفعيل حساب الطبيب." : "تم تعطيل حساب الطبيب.");
        }

        public ServiceResult QuickCheckOut(int doctorId)
        {
            var doctor = _unitOfWork.InternDoctors.GetActiveInternDoctor(doctorId);
            if (doctor == null)
            {
                return ServiceResult.Fail("Doctor not found or inactive.");
            }

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var todayAttendance = _unitOfWork.InternDoctors.GetTodayAttendance(doctorId, today, tomorrow);
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
