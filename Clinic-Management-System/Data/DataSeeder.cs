//using Bogus; // مكتبة لتوليد بيانات وهمية (installها من NuGet)
//using Clinic_Management_System.Models;
//using Clinic_Management_System.Models.Enums;
//using Microsoft.EntityFrameworkCore;

//namespace Clinic_Management_System.Data
//{
//    public static class DataSeeder
//    {
//        public static async Task SeedAsync(ApplicationDbContext context)
//        {
//            // لو فيه بيانات قبل كده، متعملش Seeding تاني
//            if (await context.Patient.AnyAsync())
//                return;

//            var random = new Random();

//            // ---------- Patients ----------
//            var patients = new Faker<Patient>("ar")
//                .RuleFor(p => p.FullName, f => f.Name.FullName())
//                .RuleFor(p => p.Address, f => f.Address.City())
//                .RuleFor(p => p.Phone, f => f.Phone.PhoneNumber("01#########"))
//                .RuleFor(p => p.Gender, f => f.PickRandom<Gender>())
//                .RuleFor(p => p.Age, f => f.Random.Int(10, 80))
//                .RuleFor(p => p.CreateAt, f => f.Date.Past(1))
//                .Generate(100);

//            await context.Patient.AddRangeAsync(patients);
//            await context.SaveChangesAsync();

//            // ---------- Receptionists ----------
//            var receptionists = new Faker<Receptionist>("ar")
//                .RuleFor(r => r.FullName, f => f.Name.FullName())
//                .RuleFor(r => r.Phone, f => f.Phone.PhoneNumber("01#########"))
//                .RuleFor(r => r.Email, f => f.Internet.Email())
//                .RuleFor(r => r.Shift, f => f.PickRandom("صباحي", "مسائي"))
//                .RuleFor(r => r.Password, f => "123456")
//                .RuleFor(r => r.Role, f => "Receptionist")
//                .RuleFor(r => r.Salary, f => f.Random.Decimal(4000, 7000))
//                .RuleFor(r => r.HiringDate, f => f.Date.Past(2))
//                .Generate(100);

//            await context.Receptionist.AddRangeAsync(receptionists);
//            await context.SaveChangesAsync();

//            // ---------- Organizations ----------
//            var orgs = new Faker<Organization>("ar")
//                .RuleFor(o => o.Name, f => $"شركة {f.Company.CompanyName()}")
//                .RuleFor(o => o.TyppeOfContract, (f, o) => new List<string> { f.PickRandom("ذهبي", "فضي", "بلاتيني") })
//                .RuleFor(o => o.CreateAt, (f, o) => f.Date.Past(3))
//                .Generate(100);

//            await context.Organizations.AddRangeAsync(orgs);
//            await context.SaveChangesAsync(); // ⬅️ مهم جدًا قبل الـ Packages

//            // ---------- Intern Doctors ----------
//            var doctors = new Faker<InternDoctor>("ar")
//                .RuleFor(d => d.FullName, f => f.Name.FullName())
//                .RuleFor(d => d.Phone, f => f.Phone.PhoneNumber("01#########"))
//                .RuleFor(d => d.Notes, f => f.Lorem.Sentence())
//                .Generate(100);

//            await context.InternDoctors.AddRangeAsync(doctors);
//            await context.SaveChangesAsync();

//            // ---------- Checks ----------
//            var checks = new Faker<Check>("ar")
//                .RuleFor(c => c.PatientId, f => f.PickRandom(patients).Id)
//                .RuleFor(c => c.Sugestion, f => f.Lorem.Sentence())
//                .RuleFor(c => c.ClinicAssessment, f => f.Lorem.Sentence())
//                .RuleFor(c => c.Diagnosis, f => f.Lorem.Sentence())
//                .RuleFor(c => c.PlaneOfTreatment, f => f.Lorem.Sentence())
//                .RuleFor(c => c.MethodsOfTreatment, f => f.Lorem.Sentence())
//                .Generate(100);

//            await context.Checks.AddRangeAsync(checks);
//            await context.SaveChangesAsync();

//            // ---------- Packages ----------
//            // IDs بعد الحفظ
//            var patientIds = patients.Select(p => p.Id).ToList();
//            var orgIds = orgs.Select(o => o.Id).ToList();
//            var doctorIds = doctors.Select(d => d.InternDoctorId).ToList();
//            var checkIds = checks.Select(c => c.CheckId).ToList();

//            var packages = new Faker<Package>("ar")
//                .RuleFor(p => p.PatientId, (f, p) => f.PickRandom(patientIds))
//                .RuleFor(p => p.OrganizationId, (f, p) => f.PickRandom(orgIds))
//                .RuleFor(p => p.InternDoctorId, (f, p) => f.PickRandom(doctorIds))
//                .RuleFor(p => p.CheckId, (f, p) => f.PickRandom(checkIds))
//                .RuleFor(p => p.Type, (f, p) => f.PickRandom<PackageType>())
//                .RuleFor(p => p.NumOfSessions, (f, p) => f.Random.Int(5, 15))
//                .RuleFor(p => p.SessionsCount, (f, p) => f.Random.Int(0, 5))
//                .RuleFor(p => p.StartDate, (f, p) => f.Date.Recent(30))
//                .RuleFor(p => p.EndDate, (f, p) => p.StartDate.AddDays(f.Random.Int(15, 60)))
//                .RuleFor(p => p.Status, (f, p) => f.PickRandom("نشط", "منتهي", "معلق"))
//                .Generate(100);

//            await context.Packages.AddRangeAsync(packages);
//            await context.SaveChangesAsync(); // ⬅️ مهم جدًا هنا

//            // ---------- Treatment Sessions ----------
//            var packageIds = packages.Select(p => p.Id).ToList();

//            var sessions = new Faker<TreatmentSession>("ar")
//                .RuleFor(t => t.PackageId, (f, t) => f.PickRandom(packageIds))
//                .RuleFor(t => t.SessionDate, (f, t) => f.Date.Recent(10))
//                .RuleFor(t => t.Prognosis, (f, t) => f.Lorem.Sentence())
//                .Generate(100);

//            await context.treatmentSessions.AddRangeAsync(sessions);
//            await context.SaveChangesAsync();

//            // ---------- Appointments ----------
//            var appointments = new Faker<Appointment>("ar")
//                .RuleFor(a => a.PatientId, (f, a) => f.PickRandom(patients).Id)
//                .RuleFor(a => a.ReceptionistId, (f, a) => f.PickRandom(receptionists).Id)
//                .RuleFor(a => a.Time, (f, a) => f.Date.Between(DateTime.Today, DateTime.Today.AddDays(1)).TimeOfDay)
//                .RuleFor(a => a.Date, (f, a) => f.Date.Future(1))
//                .RuleFor(a => a.Status, (f, a) => f.PickRandom("تم", "قيد الانتظار", "ملغي"))
//                .RuleFor(a => a.IsWithMainDoctor, (f, a) => f.Random.Bool())
//                .Generate(100);

//            await context.Appointment.AddRangeAsync(appointments);
//            await context.SaveChangesAsync();

//            // ---------- InternDoctorAttendances ----------

//            // هات IDs بتوع الدكاترة
//             doctorIds = doctors.Select(d => d.InternDoctorId).ToList();

//            var attendances = new List<InternDoctorAttendance>();

//            foreach (var docId in doctorIds)
//            {
//                var fakeAttendances = new Faker<InternDoctorAttendance>("ar")
//                    .RuleFor(a => a.InternDoctorId, docId)
//                    .RuleFor(a => a.Date, f => f.Date.Recent(100)) // آخر 100 يوم مثلاً
//                    .RuleFor(a => a.CheckIn, f => f.Date.Between(DateTime.Today.AddHours(8), DateTime.Today.AddHours(10)))
//                    .RuleFor(a => a.CheckOut, (f, a) => a.CheckIn?.AddHours(f.Random.Double(4, 8))) // خروج بعد 4 إلى 8 ساعات
//                    .RuleFor(a => a.Hours, (f, a) => (a.CheckOut - a.CheckIn)?.TotalHours)
//                    .Generate(100); // لكل دكتور 100 attendance

//                attendances.AddRange(fakeAttendances);
//            }

//            await context.Set<InternDoctorAttendance>().AddRangeAsync(attendances);
//            await context.SaveChangesAsync();

//        }
//    }
//}
