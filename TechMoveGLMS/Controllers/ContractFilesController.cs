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
    public class ContractFilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContractFilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ContractFiles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ContractFiles.Include(c => c.Contract);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ContractFiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contractFile = await _context.ContractFiles
                .Include(c => c.Contract)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contractFile == null)
            {
                return NotFound();
            }

            return View(contractFile);
        }

        // GET: ContractFiles/Create
        public IActionResult Create()
        {
            ViewData["ContractId"] = new SelectList(_context.Contracts, "Id", "ServiceLevel");
            return View();
        }

        // POST: ContractFiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ContractId,FileName,FilePath,UploadedDate")] ContractFile contractFile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contractFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ContractId"] = new SelectList(_context.Contracts, "Id", "ServiceLevel", contractFile.ContractId);
            return View(contractFile);
        }

        // GET: ContractFiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contractFile = await _context.ContractFiles.FindAsync(id);
            if (contractFile == null)
            {
                return NotFound();
            }
            ViewData["ContractId"] = new SelectList(_context.Contracts, "Id", "ServiceLevel", contractFile.ContractId);
            return View(contractFile);
        }

        // POST: ContractFiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ContractId,FileName,FilePath,UploadedDate")] ContractFile contractFile)
        {
            if (id != contractFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contractFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractFileExists(contractFile.Id))
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
            ViewData["ContractId"] = new SelectList(_context.Contracts, "Id", "ServiceLevel", contractFile.ContractId);
            return View(contractFile);
        }

        // GET: ContractFiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contractFile = await _context.ContractFiles
                .Include(c => c.Contract)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contractFile == null)
            {
                return NotFound();
            }

            return View(contractFile);
        }

        // POST: ContractFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contractFile = await _context.ContractFiles.FindAsync(id);
            if (contractFile != null)
            {
                _context.ContractFiles.Remove(contractFile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContractFileExists(int id)
        {
            return _context.ContractFiles.Any(e => e.Id == id);
        }
    }
}
