using Clinic_Management_System.Models;
using Clinic_Management_System.Services.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic_Management_System.Services.Receptionists
{
    public interface IReceptionistService
    {
        Task<List<Receptionist>> GetReceptionistsAsync();
        Task<Receptionist?> GetReceptionistByIdAsync(int? id);
        Task<Receptionist?> FindReceptionistAsync(int id);
        Task<ServiceResult> CreateReceptionistAsync(Receptionist receptionist);
        Task<ServiceResult> UpdateReceptionistAsync(int id, Receptionist receptionist);
        Task<ServiceResult> DeleteReceptionistAsync(int id);
        Task<ServiceResult<bool>> ToggleStatusAsync(int id);
    }
}
