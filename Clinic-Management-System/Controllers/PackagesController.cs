using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]
    public class PackagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PackagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Create package page
        [HttpGet]
        public IActionResult Create(int patientId)
        {
            var patient = _context.Patient
                .Include(p => p.Packages)
                .FirstOrDefault(p => p.Id == patientId);

            if (patient == null)
                return NotFound("المريض غير موجود");

            ViewBag.PatientId = patient.Id;
            ViewBag.PatientName = patient.FullName;
            ViewBag.Checks = _context.Checks.Where(c => c.PatientId == patientId).ToList();
            ViewBag.Organizations = _context.Organizations.ToList();

            // ممكن نعرض رسالة info بس بدون Redirect
            var packages = patient.Packages.ToList();
            if (!packages.Any())
            {
                TempData["Info"] = "المريض لا يملك أي باقات.";
            }
            else
            {
                var totalRemaining = packages.Sum(p => p.NumOfSessions - p.SessionsCount);
                TempData["Info"] = $"المريض لديه {totalRemaining} جلسات متبقية.";
            }

            return View(); // مهم ترجع View() هنا بدون Redirect
        }

        [HttpGet]
        public IActionResult SearchDoctors(string term)
        {
            var doctors = _context.InternDoctors
                .Where(d => d.FullName.Contains(term))
                .Select(d => new
                {
                    id = d.InternDoctorId,
                    name = d.FullName
                })
                .ToList();

            return Json(doctors);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int patientId, Package package)
        {
            // force patient id
            package.PatientId = patientId;

            // SessionsCount must start at 0 for new package
            package.SessionsCount = 0;
            package.StartDate = DateTime.Now;
            package.EndDate = null;
            package.Status = "Active";

            if (!ModelState.IsValid)
            {
                ViewBag.Organizations = _context.Organizations.ToList();
                ViewBag.Checks = _context.Checks.Where(c => c.PatientId == patientId).ToList();
                TempData["Error"] = string.Join(" | ",
                    ModelState.Values.SelectMany(v => v.Errors)
                                     .Select(e => e.ErrorMessage));
                return View(package);
            }

            // Extra business validations (NumOfSessions must be >=1 handled by annotation).
            if (package.NumOfSessions <= 0)
            {
                ModelState.AddModelError(nameof(package.NumOfSessions), "عدد الجلسات يجب أن يكون أكبر من صفر.");
                ViewBag.Organizations = _context.Organizations.ToList();
                ViewBag.Checks = _context.Checks.Where(c => c.PatientId == patientId).ToList();
                return View(package);
            }

            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            TempData["Success"] = "تمت إضافة الباقة بنجاح.";
            // بعد الإضافة نرجع لصفحة المريض (مش قايمة الباقات)
            return RedirectToAction("GetAll", "Patient");
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int id)
        {
            var package = await _context.Packages
           .Include(p => p.Patient)
           .Include(p => p.Organization)   
           .FirstOrDefaultAsync(p => p.Id == id);

            if (package == null) return NotFound();
           


            ViewBag.Doctors = _context.InternDoctors.Where(d => d.IsActive).ToList();
            ViewBag.Organizations = _context.Organizations.ToList();
            ViewBag.Checks = _context.Checks.Where(c => c.PatientId == package.PatientId).ToList();

            return View(package);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Package package)
        {
            if (id != package.Id) return NotFound();

            var existing = await _context.Packages.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (existing == null) return NotFound();

            // نضمن عدم تغيير PatientId
            package.PatientId = existing.PatientId;

            // Validation: NumOfSessions must be >= 1
            if (package.NumOfSessions <= 0)
            {
                ModelState.AddModelError(nameof(package.NumOfSessions), "عدد الجلسات يجب أن يكون أكبر من صفر.");
            }

            // Validation: cannot set total less than already used sessions
            if (package.NumOfSessions < existing.SessionsCount)
            {
                ModelState.AddModelError(nameof(package.NumOfSessions),
                    $"لا يمكن تقليل إجمالي الجلسات ({package.NumOfSessions}) أقل من الجلسات المستخدمة فعلاً ({existing.SessionsCount}).");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Doctors = _context.InternDoctors.Where(d => d.IsActive).ToList();
                ViewBag.Organizations = _context.Organizations.ToList();
                ViewBag.Checks = _context.Checks.Where(c => c.PatientId == package.PatientId).ToList();
                return View(package);
            }

            // نحافظ على SessionsCount (UsedSessions) من النسخة السابقة
            package.SessionsCount = existing.SessionsCount;

            // إعادة الحالة للنشيط دائماً بعد التعديل (حسب طلبك)
            package.Status = "Active";
            package.EndDate = null;

            // لو الجلسات المستخدمة تساوي أو أكتر من الكلي (عادة مش هتحصل لأن سبقنا الفحص)
            if (package.SessionsCount >= package.NumOfSessions)
            {
                package.SessionsCount = package.NumOfSessions;
                package.Status = "Ended";
                package.EndDate = DateTime.Now;
            }

            try
            {
                _context.Update(package);
                await _context.SaveChangesAsync();

                TempData["Success"] = "تم تعديل الباقة بنجاح.";
                // بعد التعديل نرجع لصفحة المريض
                return RedirectToAction("GetAll", "Patient");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "حدث خطأ أثناء الحفظ: " + ex.Message;
                ViewBag.Doctors = _context.InternDoctors.Where(d => d.IsActive).ToList();
                ViewBag.Organizations = _context.Organizations.ToList();
                ViewBag.Checks = _context.Checks.Where(c => c.PatientId == package.PatientId).ToList();
                return View(package);
            }
        }

        // Details
        public async Task<IActionResult> Details(int id)
        {
            var package = await _context.Packages
                .AsNoTracking()
                .Include(p => p.Patient)
                .Include(p => p.InternDoctor)
                .Include(p => p.Check)
                .Include(p => p.Organization)   
                .Include(p => p.TreatmentSessions)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (package == null) return NotFound();

            if (package.TreatmentSessions != null)
                package.TreatmentSessions = package.TreatmentSessions.OrderBy(s => s.SessionDate).ToList();

            return View(package);
        }


        // Index - (قائمة عامة إن احتجت)
        public async Task<IActionResult> Index()
        {
            var packages = await _context.Packages
                .Include(p => p.Patient)
                .Include(p => p.InternDoctor)
                .Include(p => p.Organization)
                .Include(p => p.TreatmentSessions)
                .ToListAsync();

            packages = packages
                .OrderBy(p => p.Status == "Ended")
                .ThenByDescending(p => p.StartDate)
                .ToList();

            return View(packages);
        }

        // Packages by Patient (عرض باقات المريض) — صفحة المريض نفسها ممكن تستخدم هذا الجزء
        public async Task<IActionResult> PatientPackages(int patientId)
        {
            var packages = await _context.Packages
                .Where(p => p.PatientId == patientId)
                .Include(p => p.InternDoctor)
                .Include(p => p.Organization)
                .ToListAsync();

            var patient = await _context.Patient.FindAsync(patientId);

            ViewBag.PatientName = patient?.FullName;
            ViewBag.PatientId = patientId;

            return View(packages);
        }

    }
}
