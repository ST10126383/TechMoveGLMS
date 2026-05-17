using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Data;
using TechMoveGLMS.Models;

namespace TechMoveGLMS.Controllers
{
    public class ContractsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContractsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Contracts
        public async Task<IActionResult> Index(string searchString, string status, DateTime? startDate, DateTime? endDate)
        {
            var contracts = _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.Files)
                .AsQueryable();

            // Search & Filter Logic (LINQ)
            if (!string.IsNullOrEmpty(searchString))
            {
                contracts = contracts.Where(c =>
                    c.Client.Name.Contains(searchString) ||
                    c.ServiceLevel.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(status))
            {
                contracts = contracts.Where(c => c.Status == status);
            }

            if (startDate.HasValue)
            {
                contracts = contracts.Where(c => c.StartDate >= startDate);
            }

            if (endDate.HasValue)
            {
                contracts = contracts.Where(c => c.EndDate <= endDate);
            }

            ViewBag.SearchString = searchString;
            ViewBag.Status = status;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(await contracts.ToListAsync());
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.Files)
                .Include(c => c.ServiceRequests)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contract == null) return NotFound();

            return View(contract);
        }

        // GET: Contracts/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _context.Clients
                        .OrderBy(c => c.Name)
                        .Select(c => new {
                            Id = c.Id,
                            Display = $"{c.Name} - {c.Region} ({c.ContactEmail})"
                        }),
                "Id",
                "Display");

            return View();
        }

        // POST: Contracts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClientId,StartDate,EndDate,Status,ServiceLevel")] Contract contract)
        {
            // === DIAGNOSTIC: Remove validation temporarily for testing ===
            // if (ModelState.IsValid)
            // {
            try
            {
                contract.Status = contract.Status ?? "Draft"; // Safety

                _context.Add(contract);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Contract #{contract.Id} created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error saving contract: {ex.Message}");
            }
            // }

            // If we get here, something failed
            ViewData["ClientId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _context.Clients
                        .OrderBy(c => c.Name)
                        .Select(c => new {
                            Id = c.Id,
                            Display = $"{c.Name} - {c.Region} ({c.ContactEmail})"
                        }),
                "Id",
                "Display",
                contract.ClientId);

            return View(contract);
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "ContactEmail", contract.ClientId);
            return View(contract);
        }

        // POST: Contracts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClientId,StartDate,EndDate,Status,ServiceLevel")] Contract contract)
        {
            if (id != contract.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contract);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.Id))
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
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "ContactEmail", contract.ClientId);
            return View(contract);
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract != null)
            {
                _context.Contracts.Remove(contract);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // File Upload Action
        [HttpPost]
        public async Task<IActionResult> UploadFile(int contractId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file selected");
            }

            if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only PDF files are allowed");
            }

            var uploadsFolder = Path.Combine("wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"contract_{contractId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var contractFile = new ContractFile
            {
                ContractId = contractId,
                FileName = file.FileName,
                FilePath = Path.Combine("uploads", uniqueFileName)
            };

            _context.ContractFiles.Add(contractFile);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = contractId });
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.Id == id);
        }
    }
}
