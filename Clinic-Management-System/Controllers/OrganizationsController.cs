using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]

    public class OrganizationsController : Controller
    {
        private readonly IOrganizationRepository _organizationRepository;

        public OrganizationsController(IOrganizationRepository organizationRepository)
        {
            _organizationRepository = organizationRepository;
        }

        public async Task<IActionResult> Index()
        {

            var organizations = await _organizationRepository.GetOrganizationsAsync();

            return View(organizations);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var organization = await _organizationRepository.GetOrganizationDetailsAsync(id);
            if (organization == null) return NotFound();

            return View(organization);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,CreateAt,TyppeOfContract,IsActive")] Organization organization)
        {
            if (ModelState.IsValid)
            {

                if (organization.TyppeOfContract != null)
                    organization.TyppeOfContractSerialized = string.Join(",", organization.TyppeOfContract);

                await _organizationRepository.AddOrganizationAsync(organization);

                TempData["SuccessMessage"] = "تمت إضافة الشركة بنجاح!";
                return RedirectToAction(nameof(Index));
            }
            return View(organization);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var organization = await _organizationRepository.FindOrganizationAsync(id);
            if (organization == null)
                return NotFound();

            organization.IsActive = !organization.IsActive;

            await _organizationRepository.UpdateOrganizationAsync(organization);

            TempData["EditMessage"] = organization.IsActive
                ? "تم تفعيل التعاقد بنجاح!"
                : "تم إلغاء التعاقد بنجاح!";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var organization = await _organizationRepository.FindOrganizationAsync(id.Value);
            if (organization == null) return NotFound();

            return View(organization);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CreateAt,TyppeOfContract,IsActive")] Organization organization)
        {
            if (id != organization.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {

                    if (organization.TyppeOfContract != null)
                        organization.TyppeOfContractSerialized = string.Join(",", organization.TyppeOfContract);

                    await _organizationRepository.UpdateOrganizationAsync(organization);

                    TempData["EditMessage"] = "تم تعديل بيانات الشركة بنجاح!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_organizationRepository.OrganizationExists(organization.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(organization);
        }
        [Authorize(Roles = "AdminDoctor")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var organization = await _organizationRepository.GetOrganizationForDeleteAsync(id);
            if (organization == null) return NotFound();

            return View(organization);
        }
        [Authorize(Roles = "AdminDoctor")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var organization = await _organizationRepository.FindOrganizationAsync(id);
            if (organization != null)
            {
                await _organizationRepository.RemoveOrganizationAsync(organization);

                TempData["DeleteMessage"] = "تم حذف الشركة بنجاح!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}