using Clinic_Management_System.Models;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public interface IReceptionistRepository
    {
        Task<System.Collections.Generic.List<Receptionist>> GetReceptionistsAsync();
        Task<Receptionist?> GetReceptionistByIdAsync(int? id);
        void AddReceptionist(Receptionist receptionist);
        Task<Receptionist?> FindReceptionistAsync(int id);
        void UpdateReceptionist(Receptionist receptionist);
        bool ReceptionistExists(int id);
        void RemoveReceptionist(Receptionist receptionist);
    }
}