using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories;
using Clinic_Management_System.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Services.Packages
{
    public class PackageService : IPackageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PackageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PackageFormData? GetCreateFormData(int patientId)
        {
            var patient = _unitOfWork.Packages.GetPatientWithPackages(patientId);
            if (patient == null) return null;

            var packages = (patient.Packages ?? Enumerable.Empty<Package>()).ToList();
            string infoMessage;
            if (!packages.Any())
            {
                infoMessage = "المريض لا يملك أي باقات.";
            }
            else
            {
                var totalRemaining = packages.Sum(p => p.NumOfSessions - p.SessionsCount);
                infoMessage = $"المريض لديه {totalRemaining} جلسات متبقية.";
            }

            return new PackageFormData
            {
                PatientId = patient.Id,
                PatientName = patient.FullName,
                Checks = _unitOfWork.Packages.GetChecksByPatient(patientId),
                Organizations = _unitOfWork.Packages.GetAllOrganizations(),
                InfoMessage = infoMessage
            };
        }

        public List<DoctorSearchDto> SearchDoctors(string term)
        {
            return _unitOfWork.Packages.SearchDoctors(term);
        }

        public async Task<ServiceResult> CreatePackageAsync(int patientId, Package package)
        {
            package.PatientId = patientId;
            package.SessionsCount = 0;
            package.StartDate = DateTime.Now;
            package.EndDate = null;
            package.Status = "Active";

            if (package.NumOfSessions <= 0)
            {
                return ServiceResult.Fail("عدد الجلسات يجب أن يكون أكبر من صفر.");
            }

            _unitOfWork.Packages.AddPackage(package);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Ok("تمت إضافة الباقة بنجاح.");
        }

        public async Task<PackageEditData?> GetEditFormDataAsync(int id)
        {
            var package = await _unitOfWork.Packages.GetPackageWithPatientAndOrgAsync(id);
            if (package == null) return null;

            return new PackageEditData
            {
                Package = package,
                Doctors = _unitOfWork.Packages.GetActiveInternDoctors(),
                Organizations = _unitOfWork.Packages.GetAllOrganizations(),
                Checks = _unitOfWork.Packages.GetChecksByPatient(package.PatientId)
            };
        }

        public async Task<ServiceResult> UpdatePackageAsync(int id, Package package)
        {
            if (id != package.Id)
                return ServiceResult.Fail("معرف الباقة غير متطابق.");

            var existing = await _unitOfWork.Packages.GetPackageAsNoTrackingAsync(id);
            if (existing == null)
                return ServiceResult.Fail("الباقة غير موجودة.");

            package.PatientId = existing.PatientId;

            if (package.NumOfSessions <= 0)
            {
                return ServiceResult.Fail("عدد الجلسات يجب أن يكون أكبر من صفر.");
            }

            if (package.NumOfSessions < existing.SessionsCount)
            {
                return ServiceResult.Fail($"لا يمكن تقليل إجمالي الجلسات ({package.NumOfSessions}) أقل من الجلسات المستخدمة فعلاً ({existing.SessionsCount}).");
            }

            package.SessionsCount = existing.SessionsCount;
            package.Status = "Active";
            package.EndDate = null;

            if (package.SessionsCount >= package.NumOfSessions)
            {
                package.SessionsCount = package.NumOfSessions;
                package.Status = "Ended";
                package.EndDate = DateTime.Now;
            }

            try
            {
                _unitOfWork.Packages.UpdatePackage(package);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult.Ok("تم تعديل الباقة بنجاح.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail("حدث خطأ أثناء الحفظ: " + ex.Message);
            }
        }

        public async Task<Package?> GetPackageDetailsAsync(int id)
        {
            var package = await _unitOfWork.Packages.GetPackageDetailsAsync(id);
            if (package?.TreatmentSessions != null)
            {
                package.TreatmentSessions = package.TreatmentSessions.OrderBy(s => s.SessionDate).ToList();
            }
            return package;
        }

        public async Task<List<Package>> GetAllPackagesSortedAsync()
        {
            var packages = await _unitOfWork.Packages.GetAllPackagesWithIncludesAsync();
            return packages
                .OrderBy(p => p.Status == "Ended")
                .ThenByDescending(p => p.StartDate)
                .ToList();
        }

        public async Task<(List<Package> packages, Patient? patient)> GetPatientPackagesAsync(int patientId)
        {
            var packages = await _unitOfWork.Packages.GetPatientPackagesAsync(patientId);
            var patient = await _unitOfWork.Packages.FindPatientAsync(patientId);
            return (packages, patient);
        }

        public List<Organization> GetAllOrganizations()
        {
            return _unitOfWork.Packages.GetAllOrganizations();
        }

        public List<Check> GetChecksByPatient(int patientId)
        {
            return _unitOfWork.Packages.GetChecksByPatient(patientId);
        }

        public List<InternDoctor> GetActiveInternDoctors()
        {
            return _unitOfWork.Packages.GetActiveInternDoctors();
        }
    }
}
