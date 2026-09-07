using Clinic_Management_System.Models;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public interface IReceptionistRepository
    {
        Task<System.Collections.Generic.List<Receptionist>> GetReceptionistsAsync();
        Task<Receptionist?> GetReceptionistByIdAsync(int? id);
        Task AddReceptionistAsync(Receptionist receptionist);
        Task<Receptionist?> FindReceptionistAsync(int id);
        Task UpdateReceptionistAsync(Receptionist receptionist);
        bool ReceptionistExists(int id);
        Task RemoveReceptionistAsync(Receptionist receptionist);
    }
}