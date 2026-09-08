using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories;
using Clinic_Management_System.Services.Common;
using System;
using System.Threading.Tasks;

namespace Clinic_Management_System.Services.TreatmentSessions
{
    public class TreatmentSessionService : ITreatmentSessionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TreatmentSessionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<int>> CreateSessionAsync(int patientId)
        {
            var patient = await _unitOfWork.TreatmentSessions.FindPatientAsync(patientId);
            if (patient == null)
            {
                return ServiceResult<int>.Fail("المريض غير موجود.");
            }

            var package = await _unitOfWork.TreatmentSessions.GetActivePackageByPatientAsync(patientId);
            if (package == null)
            {
                return ServiceResult<int>.Fail("❌ لا توجد باكدج متاحة لهذا المريض أو كل الجلسات خلصت.");
            }

            var session = new TreatmentSession
            {
                PackageId = package.Id,
                SessionDate = DateTime.Now,
                Prognosis = "غير محدد حالياً"
            };

            _unitOfWork.TreatmentSessions.AddTreatmentSession(session);
            package.SessionsCount++;

            if (package.SessionsCount >= package.NumOfSessions)
            {
                package.Status = "Ended";
                package.EndDate = DateTime.Now;
            }

            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<int>.Ok(patientId, "✔ تم إضافة الجلسة بنجاح.");
        }

        public async Task<TreatmentSession?> GetSessionByIdAsync(int id)
        {
            return await _unitOfWork.TreatmentSessions.GetSessionByIdAsync(id);
        }

        public async Task<TreatmentSession?> GetSessionWithPackageAsync(int id)
        {
            return await _unitOfWork.TreatmentSessions.GetSessionWithPackageAsync(id);
        }

        public async Task<TreatmentSession?> FindSessionAsync(int id)
        {
            return await _unitOfWork.TreatmentSessions.FindSessionAsync(id);
        }

        public async Task<ServiceResult<int>> EditPrognosisAsync(int id, string prognosis)
        {
            var session = await _unitOfWork.TreatmentSessions.FindSessionAsync(id);
            if (session == null)
                return ServiceResult<int>.Fail("الجلسة غير موجودة.");

            session.Prognosis = prognosis;
            _unitOfWork.TreatmentSessions.UpdateSession(session);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<int>.Ok(session.PackageId, "✔ تم تعديل التقييم بنجاح.");
        }

        public async Task<ServiceResult<int>> DeleteSessionAsync(int id)
        {
            var session = await _unitOfWork.TreatmentSessions.FindSessionAsync(id);
            if (session == null)
                return ServiceResult<int>.Fail("الجلسة غير موجودة.");

            var package = await _unitOfWork.TreatmentSessions.FindPackageAsync(session.PackageId);
            if (package != null)
            {
                if (package.SessionsCount > 0)
                {
                    package.SessionsCount -= 1;

                    if (package.SessionsCount < package.NumOfSessions)
                    {
                        package.Status = "Active";
                        package.EndDate = null;
                    }

                    _unitOfWork.TreatmentSessions.UpdatePackage(package);
                }
            }

            _unitOfWork.TreatmentSessions.RemoveSession(session);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<int>.Ok(session.PackageId, "تم حذف الجلسة بنجاح.");
        }
    }
}
