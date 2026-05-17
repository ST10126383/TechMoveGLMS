using TechMoveGLMS.Data;

namespace TechMoveGLMS.Services
{
    public class ContractService : IContractService
    {
        private readonly ApplicationDbContext _context;

        public ContractService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CanCreateServiceRequestAsync(int contractId)
        {
            var contract = await _context.Contracts
                .FindAsync(contractId);

            if (contract == null)
                return false;

            // Strong business rule validation
            var blockedStatuses = new[] { "Expired", "On Hold", "Draft" }; // Draft might also be restricted

            return !blockedStatuses.Contains(contract.Status);
        }

        // Bonus: Future method for status transition validation
        public bool IsValidStatusTransition(string currentStatus, string newStatus)
        {
            if (currentStatus == "Expired") return false;
            if (currentStatus == "On Hold" && newStatus == "Active") return true;
            return true;
        }
    }
}
