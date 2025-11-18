using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]

    public class ChecksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChecksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ عرض كل الفحوصات
        public async Task<IActionResult> Index()
        {
            var checks = await _context.Checks
                .Include(c => c.Patient)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return View(checks);
        }

        // ✅ تفاصيل فحص محدد
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var check = await _context.Checks
                .Include(c => c.Patient)
                .FirstOrDefaultAsync(c => c.CheckId == id);

            if (check == null)
                return NotFound();

            return View(check);
        }

        // ✅ إنشاء فحص جديد (GET)
        public IActionResult Create(int? patientId)
        {
            if (patientId.HasValue)
            {
                var patient = _context.Patient.FirstOrDefault(p => p.Id == patientId.Value);
                ViewBag.PatientName = patient?.FullName;

                var check = new Check
                {
                    PatientId = patientId.Value,
                    CreatedAt = DateTime.Now
                };
                return View(check);
            }

            ViewData["PatientId"] = new SelectList(_context.Patient, "Id", "FullName");
            return View();
        }

        // ✅ إنشاء فحص جديد (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CheckId,PatientId,Sugestion,ClinicAssessment,Diagnosis,PlaneOfTreatment,MethodsOfTreatment")] Check check)
        {
            // تأكد أن المريض تم تمريره
            if (check.PatientId == 0)
            {
                ModelState.AddModelError("PatientId", "يجب اختيار المريض قبل إنشاء الكشف.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["PatientId"] = new SelectList(_context.Patient, "Id", "FullName", check.PatientId);
                return View(check);
            }

            check.CreatedAt = DateTime.Now;
            _context.Checks.Add(check);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ✅ تعديل فحص موجود
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var check = await _context.Checks
                .Include(c => c.Patient)
                .FirstOrDefaultAsync(c => c.CheckId == id);

            if (check == null)
                return NotFound();

            ViewBag.PatientName = check.Patient?.FullName;
            return View(check);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CheckId,PatientId,Sugestion,ClinicAssessment,Diagnosis,PlaneOfTreatment,MethodsOfTreatment,CreatedAt")] Check check)
        {
            if (id != check.CheckId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(check);

            try
            {
                _context.Update(check);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Checks.Any(e => e.CheckId == check.CheckId))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // ✅ حذف فحص
        [Authorize(Roles = "AdminDoctor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var check = await _context.Checks
                .Include(c => c.Patient)
                .FirstOrDefaultAsync(m => m.CheckId == id);

            if (check == null)
                return NotFound();

            ViewBag.PatientName = check.Patient?.FullName;
            return View(check);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var check = await _context.Checks.FindAsync(id);
            if (check != null)
            {
                _context.Checks.Remove(check);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
