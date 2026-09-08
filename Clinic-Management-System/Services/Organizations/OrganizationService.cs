using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories;
using Clinic_Management_System.Services.Common;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic_Management_System.Services.Organizations
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrganizationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Organization>> GetOrganizationsAsync()
        {
            return await _unitOfWork.Organizations.GetOrganizationsAsync();
        }

        public async Task<Organization?> GetOrganizationDetailsAsync(int? id)
        {
            if (id == null) return null;
            return await _unitOfWork.Organizations.GetOrganizationDetailsAsync(id);
        }

        public async Task<ServiceResult> CreateOrganizationAsync(Organization organization)
        {
            if (organization.TyppeOfContract != null)
            {
                organization.TyppeOfContractSerialized = string.Join(",", organization.TyppeOfContract);
            }

            _unitOfWork.Organizations.AddOrganization(organization);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Ok("تمت إضافة الشركة بنجاح!");
        }

        public async Task<Organization?> FindOrganizationAsync(int id)
        {
            return await _unitOfWork.Organizations.FindOrganizationAsync(id);
        }

        public async Task<Organization?> GetOrganizationForDeleteAsync(int? id)
        {
            return await _unitOfWork.Organizations.GetOrganizationForDeleteAsync(id);
        }

        public async Task<ServiceResult> UpdateOrganizationAsync(int id, Organization organization)
        {
            if (id != organization.Id)
            {
                return ServiceResult.Fail("معرف الشركة غير متطابق.");
            }

            try
            {
                if (organization.TyppeOfContract != null)
                {
                    organization.TyppeOfContractSerialized = string.Join(",", organization.TyppeOfContract);
                }

                _unitOfWork.Organizations.UpdateOrganization(organization);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult.Ok("تم تعديل بيانات الشركة بنجاح!");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_unitOfWork.Organizations.OrganizationExists(organization.Id))
                {
                    return ServiceResult.Fail("الشركة غير موجودة.");
                }
                throw;
            }
        }

        public async Task<ServiceResult<bool>> ToggleStatusAsync(int id)
        {
            var organization = await _unitOfWork.Organizations.FindOrganizationAsync(id);
            if (organization == null)
            {
                return ServiceResult<bool>.Fail("الشركة غير موجودة.");
            }

            organization.IsActive = !organization.IsActive;
            _unitOfWork.Organizations.UpdateOrganization(organization);
            await _unitOfWork.SaveChangesAsync();

            var msg = organization.IsActive ? "تم تفعيل التعاقد بنجاح!" : "تم إلغاء التعاقد بنجاح!";
            return ServiceResult<bool>.Ok(organization.IsActive, msg);
        }

        public async Task<ServiceResult> DeleteOrganizationAsync(int id)
        {
            var organization = await _unitOfWork.Organizations.FindOrganizationAsync(id);
            if (organization == null)
            {
                return ServiceResult.Fail("الشركة غير موجودة.");
            }

            _unitOfWork.Organizations.RemoveOrganization(organization);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Ok("تم حذف الشركة بنجاح!");
        }
    }
}
