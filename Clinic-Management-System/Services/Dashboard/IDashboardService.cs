using Clinic_Management_System.Models.ViewModels;
using System.Threading.Tasks;

namespace Clinic_Management_System.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardMetricsAsync();
    }
}
