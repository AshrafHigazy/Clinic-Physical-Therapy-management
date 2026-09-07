using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public interface IUnitOfWork
    {
        IAppointmentRepository Appointments { get; }
        ICheckRepository Checks { get; }
        IDashboardRepository Dashboard { get; }
        IInternDoctorRepository InternDoctors { get; }
        IInternDoctorAttendanceRepository InternDoctorAttendances { get; }
        IOrganizationRepository Organizations { get; }
        IPackageRepository Packages { get; }
        IPatientRepository Patients { get; }
        IReceptionistRepository Receptionists { get; }
        IReceptionistAttendanceRepository ReceptionistAttendances { get; }
        ITreatmentSessionRepository TreatmentSessions { get; }

        int SaveChanges();
        Task<int> SaveChangesAsync();

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}