using Clinic_Management_System.Models;

namespace Clinic_Management_System.Repositories
{
    public interface IInternDoctorAttendanceRepository
    {
        System.Collections.Generic.List<InternDoctorAttendance> GetAttendancesByDate(System.DateTime selectedDate);
        System.Collections.Generic.List<InternDoctor> GetActiveInternDoctors();
        InternDoctor? GetActiveInternDoctor(int id);
        bool AttendanceExists(int doctorId, System.DateTime date);
        void AddAttendance(InternDoctorAttendance attendance);
        InternDoctor? GetInternDoctorWithAttendances(int id);
        InternDoctorAttendance? GetTodayAttendance(int doctorId, System.DateTime today, System.DateTime tomorrow);
    }
}