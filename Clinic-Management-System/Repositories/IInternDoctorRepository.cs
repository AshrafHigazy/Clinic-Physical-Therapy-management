using Clinic_Management_System.Models;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public interface IInternDoctorRepository
    {
        Task<List<InternDoctor>> GetDoctorsAsync(string? search);
        Task AddDoctorAsync(InternDoctor doctor);
        InternDoctor? GetDoctorWithAttendances(int id);
        Task<InternDoctor?> GetDoctorByFilterAsync(int id);
        Task<InternDoctor?> FindDoctorAsync(int id);
        Task UpdateDoctorAsync(InternDoctor doctor);
        Task RemoveDoctorAsync(InternDoctor doctor);
        InternDoctor? GetActiveInternDoctor(int id);
        InternDoctorAttendance? GetTodayAttendance(int doctorId, System.DateTime today, System.DateTime tomorrow);
        void SaveChanges();
    }
}