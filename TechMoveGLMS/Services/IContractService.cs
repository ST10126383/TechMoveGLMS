namespace TechMoveGLMS.Services
{
    public interface IContractService
    {
        Task<bool> CanCreateServiceRequestAsync(int contractId);
    }
}
