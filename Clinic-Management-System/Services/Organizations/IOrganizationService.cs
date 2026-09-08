using Clinic_Management_System.Models;
using Clinic_Management_System.Services.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic_Management_System.Services.Organizations
{
    public interface IOrganizationService
    {
        Task<List<Organization>> GetOrganizationsAsync();
        Task<Organization?> GetOrganizationDetailsAsync(int? id);
        Task<ServiceResult> CreateOrganizationAsync(Organization organization);
        Task<Organization?> FindOrganizationAsync(int id);
        Task<Organization?> GetOrganizationForDeleteAsync(int? id);
        Task<ServiceResult> UpdateOrganizationAsync(int id, Organization organization);
        Task<ServiceResult<bool>> ToggleStatusAsync(int id);
        Task<ServiceResult> DeleteOrganizationAsync(int id);
    }
}
