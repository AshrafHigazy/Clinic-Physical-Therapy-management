using Clinic_Management_System.Models;
using Clinic_Management_System.Services.Organizations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]
    public class OrganizationsController : Controller
    {
        private readonly IOrganizationService _organizationService;

        public OrganizationsController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        public async Task<IActionResult> Index()
        {
            var organizations = await _organizationService.GetOrganizationsAsync();
            return View(organizations);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var organization = await _organizationService.GetOrganizationDetailsAsync(id);
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
                var result = await _organizationService.CreateOrganizationAsync(organization);
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(organization);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = await _organizationService.ToggleStatusAsync(id);
            if (!result.Success)
                return NotFound();

            TempData["EditMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var organization = await _organizationService.FindOrganizationAsync(id.Value);
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
                var result = await _organizationService.UpdateOrganizationAsync(id, organization);
                if (!result.Success)
                    return NotFound();

                TempData["EditMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(organization);
        }

        [Authorize(Roles = "AdminDoctor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var organization = await _organizationService.GetOrganizationForDeleteAsync(id);
            if (organization == null) return NotFound();

            return View(organization);
        }

        [Authorize(Roles = "AdminDoctor")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _organizationService.DeleteOrganizationAsync(id);
            if (result.Success)
            {
                TempData["DeleteMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}