using Clinic_Management_System.Models;
using Clinic_Management_System.Services.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic_Management_System.Services.InternDoctors
{
    public interface IInternDoctorService
    {
        Task<List<InternDoctor>> GetDoctorsAsync(string? search);
        Task<ServiceResult> CreateDoctorAsync(InternDoctor doctor);
        InternDoctor? GetDoctorWithAttendances(int id);
        Task<InternDoctor?> FindDoctorAsync(int id);
        Task<InternDoctor?> GetDoctorForDeleteAsync(int id);
        Task<ServiceResult> UpdateDoctorAsync(int id, InternDoctor doctor);
        Task<ServiceResult> DeleteDoctorAsync(int id);
        Task<ServiceResult> ToggleActiveAsync(int id);
        ServiceResult QuickCheckOut(int doctorId);
    }
}
