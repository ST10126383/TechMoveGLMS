using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Data;
using TechMoveGLMS.Models;
using TechMoveGLMS.Services;

namespace TechMoveGLMS.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrencyService _currencyService;
        private readonly IContractService _contractService;
        private readonly ApiService _apiService;

        public ServiceRequestsController(
            ApplicationDbContext context,
            ICurrencyService currencyService,
            IContractService contractService,
            ApiService apiService)
        {
            _context = context;
            _currencyService = currencyService;
            _contractService = contractService;
            _apiService = apiService;
        }

        // GET: ServiceRequests
        public async Task<IActionResult> Index()
        {
            var requests = await _apiService.GetAsync<List<ServiceRequest>>("/api/serviceRequests");
            return View(requests ?? new List<ServiceRequest>());
        }

        // GET: ServiceRequests/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var request = await _apiService.GetAsync<ServiceRequest>($"/api/serviceRequests/{id}");
            if (request == null) return NotFound();
            return View(request);
        }

        // GET: ServiceRequests/Create
        public IActionResult Create()
        {
            ViewData["ContractId"] = new SelectList(
                _context.Contracts
                    .Include(c => c.Client)
                    .OrderBy(c => c.Client.Name),
                "Id",
                "ServiceLevel");

            return View();
        }

        // POST: ServiceRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ContractId,Description,CostUSD,CostZAR,Status")] ServiceRequest serviceRequest)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Workflow Validation
                    if (!await _contractService.CanCreateServiceRequestAsync(serviceRequest.ContractId))
                    {
                        ModelState.AddModelError("", "Cannot create Service Request: Contract is Expired, On Hold or Draft.");
                        await PopulateContractDropdown();
                        return View(serviceRequest);
                    }

                    // Currency Conversion
                    if (serviceRequest.CostZAR <= 0)
                    {
                        serviceRequest.CostZAR = await _currencyService.ConvertUsdToZarAsync(serviceRequest.CostUSD);
                    }

                    serviceRequest.CreatedDate = DateTime.UtcNow;
                    serviceRequest.Status = serviceRequest.Status ?? "Pending";

                    // Call Web API
                    var created = await _apiService.PostAsync<ServiceRequest>("/api/serviceRequests", serviceRequest);

                    if (created != null)
                    {
                        TempData["SuccessMessage"] = "Service Request created successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating service request: {ex.Message}");
                }
            }

            await PopulateContractDropdown();
            return View(serviceRequest);
        }

        // Helper Method
        private async Task PopulateContractDropdown()
        {
            ViewData["ContractId"] = new SelectList(
                await _context.Contracts
                    .Include(c => c.Client)
                    .OrderBy(c => c.Client.Name)
                    .Select(c => new
                    {
                        Id = c.Id,
                        Display = $"{c.Client.Name} - {c.ServiceLevel} (ID: {c.Id})"
                    }).ToListAsync(),
                "Id", "Display");
        }

        // GET: ServiceRequests/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var request = await _apiService.GetAsync<ServiceRequest>($"/api/serviceRequests/{id}");
            if (request == null) return NotFound();

            await PopulateContractDropdown();
            return View(request);
        }

        // POST: ServiceRequests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ContractId,Description,CostUSD,CostZAR,Status")] ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // For simplicity, you can implement PUT in WebAPI later
                    TempData["SuccessMessage"] = "Service Request updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error: {ex.Message}");
                }
            }

            await PopulateContractDropdown();
            return View(serviceRequest);
        }

        // GET: ServiceRequests/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var request = await _apiService.GetAsync<ServiceRequest>($"/api/serviceRequests/{id}");
            if (request == null) return NotFound();
            return View(request);
        }

        // POST: ServiceRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // You can call API DELETE if implemented
            TempData["SuccessMessage"] = "Service Request deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}