using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public ServiceRequestsController(ApplicationDbContext context, ICurrencyService currencyService,IContractService contractService)
        {
            _context = context;
            _currencyService = currencyService;
            _contractService = contractService;
        }

        // GET: ServiceRequests
        public async Task<IActionResult> Index()
        {
            var serviceRequests = await _context.ServiceRequests
                .Include(s => s.Contract)
                .ThenInclude(c => c.Client)
                .ToListAsync();
            return View(serviceRequests);
        }

        // GET: ServiceRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        // GET: ServiceRequests/Create
        public IActionResult Create()
        {
            ViewData["ContractId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _context.Contracts.Include(c => c.Client), "Id", "Id"); // You can improve display later
            return View();
        }

        // POST: ServiceRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ContractId,Description,CostUSD,CostZAR,Status")] ServiceRequest serviceRequest)
        {
            if (ModelState.IsValid)
            {
                if (!await _contractService.CanCreateServiceRequestAsync(serviceRequest.ContractId))
                {
                    ModelState.AddModelError("", "Cannot create Service Request: Contract is Expired, On Hold, or Draft.");
                    await PopulateContractDropdown();
                    return View(serviceRequest);
                }

                // Use form value if provided, otherwise calculate
                if (serviceRequest.CostZAR == 0)
                {
                    serviceRequest.CostZAR = await _currencyService.ConvertUsdToZarAsync(serviceRequest.CostUSD);
                }

                serviceRequest.CreatedDate = DateTime.UtcNow;
                serviceRequest.Status = serviceRequest.Status ?? "Pending";

                _context.Add(serviceRequest);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Service Request created successfully!";
                return RedirectToAction(nameof(Index));
            }

            await PopulateContractDropdown();
            return View(serviceRequest);
        }

        // Helper method
        private async Task PopulateContractDropdown()
        {
            ViewData["ContractId"] = new SelectList(
                await _context.Contracts
                    .Include(c => c.Client)
                    .OrderBy(c => c.Client.Name)
                    .Select(c => new {
                        Id = c.Id,
                        Display = $"{c.Client.Name} - {c.ServiceLevel} (ID: {c.Id})"
                    }).ToListAsync(),
                "Id", "Display");
        }

        // GET: ServiceRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest == null)
            {
                return NotFound();
            }
            ViewData["ContractId"] = new SelectList(_context.Contracts, "Id", "ServiceLevel", serviceRequest.ContractId);
            return View(serviceRequest);
        }

        // POST: ServiceRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ContractId,Description,CostUSD,CostZAR,Status,CreatedDate")] ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceRequestExists(serviceRequest.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ContractId"] = new SelectList(_context.Contracts, "Id", "ServiceLevel", serviceRequest.ContractId);
            return View(serviceRequest);
        }

        // GET: ServiceRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        // POST: ServiceRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest != null)
            {
                _context.ServiceRequests.Remove(serviceRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceRequestExists(int id)
        {
            return _context.ServiceRequests.Any(e => e.Id == id);
        }
    }
}
