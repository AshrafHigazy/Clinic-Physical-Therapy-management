using Clinic_Management_System.Models;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public interface IOrganizationRepository
    {
        Task<System.Collections.Generic.List<Organization>> GetOrganizationsAsync();
        Task<Organization?> GetOrganizationDetailsAsync(int? id);
        void AddOrganization(Organization organization);
        Task<Organization?> FindOrganizationAsync(int id);
        void UpdateOrganization(Organization organization);
        void RemoveOrganization(Organization organization);
        bool OrganizationExists(int id);
        Task<Organization?> GetOrganizationForDeleteAsync(int? id);
    }
}