using Clinic_Management_System.Models;
using System.Collections.Generic;

namespace Clinic_Management_System.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalPatients { get; set; }
        public int TodayAppointmentsCount { get; set; }
        public int TodayAttended { get; set; }
        public int TodayCanceled { get; set; }
        public int ActivePackagesCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public int[] WeeklyAppointments { get; set; } = new int[7];
        public int[] PackageDistribution { get; set; } = new int[5];
        public int[] MonthlyDataCounts { get; set; } = new int[12];
        public decimal[] MonthlyDataRevenues { get; set; } = new decimal[12];
        public int CurrentMonthIndex { get; set; }
        public decimal TodayPackagesSum { get; set; }
        public int TodayPackagesCount { get; set; }
        public int TodayChecksCount { get; set; }
        public int TodayNewPatientsCount { get; set; }
        public List<Package> TodayPackagesList { get; set; } = new();
        public List<Check> TodayChecksList { get; set; } = new();
        public decimal TodayTotalEarnings { get; set; }
        public List<Patient> RecentPatients { get; set; } = new();
        public List<Appointment> TodayAppointmentsList { get; set; } = new();
        public int ActiveDoctors { get; set; }
        public int ActiveOrganizations { get; set; }
        public int ActiveReceptionists { get; set; }
        public int ActivePackagesCountStat { get; set; }
        public int ExpiredPackagesCount { get; set; }
        public int RemainingSessionsOverall { get; set; }
    }
}
