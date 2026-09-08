using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories;
using Clinic_Management_System.Services.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic_Management_System.Services.Receptionists
{
    public class ReceptionistService : IReceptionistService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReceptionistService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Receptionist>> GetReceptionistsAsync()
        {
            return await _unitOfWork.Receptionists.GetReceptionistsAsync();
        }

        public async Task<Receptionist?> GetReceptionistByIdAsync(int? id)
        {
            if (id == null) return null;
            return await _unitOfWork.Receptionists.GetReceptionistByIdAsync(id);
        }

        public async Task<Receptionist?> FindReceptionistAsync(int id)
        {
            return await _unitOfWork.Receptionists.FindReceptionistAsync(id);
        }

        public async Task<ServiceResult> CreateReceptionistAsync(Receptionist receptionist)
        {
            receptionist.HiringDate = DateTime.Now;
            receptionist.IsActive = true;
            receptionist.AppointmentLinks = new List<AppointmentByPatientOrReceptionist>();

            _unitOfWork.Receptionists.AddReceptionist(receptionist);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Ok("تمت إضافة موظف الاستقبال بنجاح!");
        }

        public async Task<ServiceResult> UpdateReceptionistAsync(int id, Receptionist receptionist)
        {
            if (id != receptionist.Id)
            {
                return ServiceResult.Fail("معرف الموظف غير متطابق.");
            }

            try
            {
                _unitOfWork.Receptionists.UpdateReceptionist(receptionist);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult.Ok("تم تعديل بيانات موظف الاستقبال بنجاح!");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_unitOfWork.Receptionists.ReceptionistExists(receptionist.Id))
                {
                    return ServiceResult.Fail("موظف الاستقبال غير موجود.");
                }
                throw;
            }
        }

        public async Task<ServiceResult> DeleteReceptionistAsync(int id)
        {
            var receptionist = await _unitOfWork.Receptionists.FindReceptionistAsync(id);
            if (receptionist == null)
            {
                return ServiceResult.Fail("موظف الاستقبال غير موجود.");
            }

            _unitOfWork.Receptionists.RemoveReceptionist(receptionist);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult.Ok("تم حذف موظف الاستقبال بنجاح!");
        }

        public async Task<ServiceResult<bool>> ToggleStatusAsync(int id)
        {
            var receptionist = await _unitOfWork.Receptionists.FindReceptionistAsync(id);
            if (receptionist == null)
            {
                return ServiceResult<bool>.Fail("موظف الاستقبال غير موجود.");
            }

            receptionist.IsActive = !receptionist.IsActive;
            _unitOfWork.Receptionists.UpdateReceptionist(receptionist);
            await _unitOfWork.SaveChangesAsync();

            var msg = receptionist.IsActive ? "تم تفعيل الموظف بنجاح!" : "تم تعطيل الموظف بنجاح!";
            return ServiceResult<bool>.Ok(receptionist.IsActive, msg);
        }
    }
}
