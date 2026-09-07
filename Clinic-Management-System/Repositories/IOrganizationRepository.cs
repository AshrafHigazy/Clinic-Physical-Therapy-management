using Clinic_Management_System.Models;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public interface IOrganizationRepository
    {
        Task<System.Collections.Generic.List<Organization>> GetOrganizationsAsync();
        Task<Organization?> GetOrganizationDetailsAsync(int? id);
        Task AddOrganizationAsync(Organization organization);
        Task<Organization?> FindOrganizationAsync(int id);
        Task UpdateOrganizationAsync(Organization organization);
        Task RemoveOrganizationAsync(Organization organization);
        bool OrganizationExists(int id);
        Task<Organization?> GetOrganizationForDeleteAsync(int? id);
    }
}