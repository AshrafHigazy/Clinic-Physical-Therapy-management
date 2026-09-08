using Clinic_Management_System.Models;
using Clinic_Management_System.Models.ViewModels;
using Clinic_Management_System.Services.Common;
using System;
using System.Collections.Generic;

namespace Clinic_Management_System.Services.InternDoctorAttendances
{
    public interface IInternDoctorAttendanceService
    {
        List<InternDoctorAttendance> GetAttendancesByDate(DateTime selectedDate);
        List<InternDoctor> GetActiveInternDoctors();
        ServiceResult RecordAttendance(InternDoctorAttendance attendance);
        InternDoctor? GetDoctorWithAttendances(int id);
        DoctorAttendanceReport? GetDoctorAttendanceReport(int id, DateTime? from, DateTime? to);
        ServiceResult QuickCheckIn(int doctorId);
        ServiceResult QuickCheckOut(int doctorId);
    }
}
