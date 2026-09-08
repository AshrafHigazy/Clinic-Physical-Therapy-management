using Clinic_Management_System.Models;
using System;
using System.Collections.Generic;

namespace Clinic_Management_System.Models.ViewModels
{
    public class DoctorAttendanceReport
    {
        public string DoctorName { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public double TotalHours { get; set; }
        public double AvgHours { get; set; }
        public List<InternDoctorAttendance> Attendances { get; set; } = new();
    }
}
