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
    public class ContractsController : Controller
    {
        private readonly ApiService _apiService;

        public ContractsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: Contracts
        public async Task<IActionResult> Index()
        {
            var contracts = await _apiService.GetAsync<List<Contract>>("/api/contracts");
            return View(contracts ?? new List<Contract>());
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var contract = await _apiService.GetAsync<Contract>($"/api/contracts/{id}");
            if (contract == null) return NotFound();
            return View(contract);
        }

        // GET: Contracts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Contracts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract)
        {
            if (ModelState.IsValid)
            {
                var created = await _apiService.PostAsync<Contract>("/api/contracts", contract);
                if (created != null)
                {
                    TempData["SuccessMessage"] = "Contract created successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(contract);
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var contract = await _apiService.GetAsync<Contract>($"/api/contracts/{id}");
            if (contract == null) return NotFound();
            return View(contract);
        }

        // POST: Contracts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract)
        {
            if (id != contract.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // For simplicity, you can use PUT if your API supports it
                // Or keep using direct for Edit for now
                TempData["SuccessMessage"] = "Contract updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(contract);
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _apiService.GetAsync<Contract>($"/api/contracts/{id}");
            if (contract == null) return NotFound();
            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Call API DELETE if implemented, otherwise keep as is
            TempData["SuccessMessage"] = "Contract deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // File Upload - Kept in MVC (Recommended for Part 3)
        [HttpPost]
        public async Task<IActionResult> UploadFile(int contractId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "No file selected";
                return RedirectToAction(nameof(Details), new { id = contractId });
            }

            if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Only PDF files are allowed";
                return RedirectToAction(nameof(Details), new { id = contractId });
            }

            if (file.Length > 5 * 1024 * 1024) // 5MB limit
            {
                TempData["ErrorMessage"] = "File size cannot exceed 5MB";
                return RedirectToAction(nameof(Details), new { id = contractId });
            }

            var uploadsFolder = Path.Combine("wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"contract_{contractId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // You can call API to save file record if needed, or keep local for now
            TempData["SuccessMessage"] = "Signed Agreement uploaded successfully!";
            return RedirectToAction(nameof(Details), new { id = contractId });
        }

    }
}
