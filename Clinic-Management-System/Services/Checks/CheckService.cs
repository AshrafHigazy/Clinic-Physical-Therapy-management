using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories;
using Clinic_Management_System.Services.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic_Management_System.Services.Checks
{
    public class CheckService : ICheckService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Check>> GetChecksAsync()
        {
            return await _unitOfWork.Checks.GetChecksAsync();
        }

        public async Task<Check?> GetCheckDetailsAsync(int? id)
        {
            if (id == null) return null;
            return await _unitOfWork.Checks.GetCheckDetailsAsync(id);
        }

        public Patient? GetPatientById(int id)
        {
            return _unitOfWork.Checks.GetPatientById(id);
        }

        public List<Patient> GetPatientsForSelect()
        {
            return _unitOfWork.Checks.GetPatientsForSelect();
        }

        public async Task<ServiceResult> CreateCheckAsync(Check check)
        {
            if (check.PatientId == 0)
            {
                return ServiceResult.Fail("يجب اختيار المريض قبل إنشاء الكشف.");
            }

            check.CreatedAt = DateTime.Now;
            _unitOfWork.Checks.AddCheck(check);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Ok("تم إنشاء الكشف بنجاح.");
        }

        public async Task<Check?> GetCheckForEditAsync(int? id)
        {
            if (id == null) return null;
            return await _unitOfWork.Checks.GetCheckForEditAsync(id);
        }

        public async Task<ServiceResult> UpdateCheckAsync(int id, Check check)
        {
            if (id != check.CheckId)
            {
                return ServiceResult.Fail("معرف الكشف غير متطابق.");
            }

            try
            {
                _unitOfWork.Checks.UpdateCheck(check);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult.Ok("تم تعديل الكشف بنجاح.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_unitOfWork.Checks.CheckExists(check.CheckId))
                {
                    return ServiceResult.Fail("الكشف غير موجود.");
                }
                throw;
            }
        }

        public async Task<ServiceResult> DeleteCheckAsync(int id)
        {
            var check = await _unitOfWork.Checks.FindCheckAsync(id);
            if (check == null)
            {
                return ServiceResult.Fail("الكشف غير موجود.");
            }

            _unitOfWork.Checks.RemoveCheck(check);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult.Ok("تم حذف الكشف بنجاح.");
        }
    }
}
