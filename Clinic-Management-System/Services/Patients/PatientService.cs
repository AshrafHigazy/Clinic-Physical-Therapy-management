using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories;
using Clinic_Management_System.Services.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Clinic_Management_System.Services.Patients
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PatientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<Patient> GetPatients(string? searchString, string? sortOrder)
        {
            return _unitOfWork.Patients.GetPatients(searchString, sortOrder ?? "");
        }

        public Patient? GetPatientById(int? id)
        {
            if (id == null || id == 0) return null;
            return _unitOfWork.Patients.GetPatientById(id);
        }

        public Patient? FindPatient(int? id)
        {
            if (id == null || id == 0) return null;
            return _unitOfWork.Patients.FindPatient(id);
        }

        public ServiceResult CreatePatient(Patient patient)
        {
            patient.CreateAt = DateTime.Now;
            _unitOfWork.Patients.AddPatient(patient);
            _unitOfWork.SaveChanges();
            return ServiceResult.Ok("تمت إضافة المريض بنجاح!");
        }

        public ServiceResult UpdatePatient(int id, Patient patientFromRq)
        {
            if (id != patientFromRq.Id)
            {
                return ServiceResult.Fail("معرف المريض غير متطابق.");
            }

            try
            {
                var patientDB = _unitOfWork.Patients.GetPatientById(id);
                if (patientDB == null)
                {
                    return ServiceResult.Fail("المريض غير موجود.");
                }

                patientDB.FullName = patientFromRq.FullName;
                patientDB.Address = patientFromRq.Address;
                patientDB.Age = patientFromRq.Age;
                patientDB.CreateAt = patientFromRq.CreateAt;
                patientDB.Phone = patientFromRq.Phone;
                patientDB.Gender = patientFromRq.Gender;
                patientDB.MedicalRecords = patientFromRq.MedicalRecords;

                _unitOfWork.Patients.UpdatePatient(patientDB);
                _unitOfWork.SaveChanges();

                return ServiceResult.Ok("تم تعديل بيانات المريض بنجاح!");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_unitOfWork.Patients.PatientExists(id))
                {
                    return ServiceResult.Fail("المريض غير موجود.");
                }
                throw;
            }
        }

        public ServiceResult DeletePatient(int? id)
        {
            if (id == null || id == 0)
            {
                return ServiceResult.Fail("معرف المريض غير صالح.");
            }

            var patient = _unitOfWork.Patients.GetPatientById(id);
            if (patient == null)
            {
                return ServiceResult.Fail("المريض غير موجود.");
            }

            _unitOfWork.Patients.RemovePatient(patient);
            _unitOfWork.SaveChanges();
            return ServiceResult.Ok("تم حذف المريض بنجاح.");
        }
    }
}
