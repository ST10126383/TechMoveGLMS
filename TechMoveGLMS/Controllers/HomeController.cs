using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Data;
using TechMoveGLMS.Models;

namespace TechMoveGLMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Dashboard Statistics
            ViewBag.TotalContracts = await _context.Contracts.CountAsync();
            ViewBag.TotalRequests = await _context.ServiceRequests.CountAsync();
            ViewBag.PendingRequests = await _context.ServiceRequests
                .CountAsync(s => s.Status != "Completed");

            ViewBag.TotalRevenue = await _context.ServiceRequests
                .SumAsync(s => s.CostUSD);

            // Recent Contracts (Last 5)
            ViewBag.RecentContracts = await _context.Contracts
                .Include(c => c.Client)
                .OrderByDescending(c => c.Id)
                .Take(5)
                .ToListAsync();

            // Recent Service Requests (Last 5)
            ViewBag.RecentRequests = await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .OrderByDescending(sr => sr.CreatedDate)
                .Take(5)
                .ToListAsync();

            ViewBag.TotalRevenue = await _context.ServiceRequests.SumAsync(s => s.CostUSD);

            return View();
        }
    }
}