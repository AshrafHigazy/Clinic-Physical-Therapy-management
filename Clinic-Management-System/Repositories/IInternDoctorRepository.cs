using Clinic_Management_System.Models;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public interface IInternDoctorRepository
    {
        Task<List<InternDoctor>> GetDoctorsAsync(string? search);
        void AddDoctor(InternDoctor doctor);
        InternDoctor? GetDoctorWithAttendances(int id);
        Task<InternDoctor?> GetDoctorByFilterAsync(int id);
        Task<InternDoctor?> FindDoctorAsync(int id);
        void UpdateDoctor(InternDoctor doctor);
        void RemoveDoctor(InternDoctor doctor);
        InternDoctor? GetActiveInternDoctor(int id);
        InternDoctorAttendance? GetTodayAttendance(int doctorId, System.DateTime today, System.DateTime tomorrow);
    }
}