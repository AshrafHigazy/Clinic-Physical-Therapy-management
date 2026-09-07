using Clinic_Management_System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Clinic_Management_System.Repositories
{
    public class UnitOfWork : IUnitOfWork, IAsyncDisposable
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _currentTransaction;

        public IAppointmentRepository Appointments { get; }
        public ICheckRepository Checks { get; }
        public IDashboardRepository Dashboard { get; }
        public IInternDoctorRepository InternDoctors { get; }
        public IInternDoctorAttendanceRepository InternDoctorAttendances { get; }
        public IOrganizationRepository Organizations { get; }
        public IPackageRepository Packages { get; }
        public IPatientRepository Patients { get; }
        public IReceptionistRepository Receptionists { get; }
        public IReceptionistAttendanceRepository ReceptionistAttendances { get; }
        public ITreatmentSessionRepository TreatmentSessions { get; }

        public UnitOfWork(
            ApplicationDbContext context,
            IAppointmentRepository appointments,
            ICheckRepository checks,
            IDashboardRepository dashboard,
            IInternDoctorRepository internDoctors,
            IInternDoctorAttendanceRepository internDoctorAttendances,
            IOrganizationRepository organizations,
            IPackageRepository packages,
            IPatientRepository patients,
            IReceptionistRepository receptionists,
            IReceptionistAttendanceRepository receptionistAttendances,
            ITreatmentSessionRepository treatmentSessions)
        {
            _context = context;
            Appointments = appointments;
            Checks = checks;
            Dashboard = dashboard;
            InternDoctors = internDoctors;
            InternDoctorAttendances = internDoctorAttendances;
            Organizations = organizations;
            Packages = packages;
            Patients = patients;
            Receptionists = receptionists;
            ReceptionistAttendances = receptionistAttendances;
            TreatmentSessions = treatmentSessions;
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                throw new InvalidOperationException(
                    "A transaction is already active for this Unit of Work.");
            }

            _currentTransaction =
                await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException(
                    "No active transaction exists.");
            }

            try
            {
                await _currentTransaction.CommitAsync();
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException(
                    "No active transaction exists.");
            }

            try
            {
                await _currentTransaction.RollbackAsync();
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }
}