using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Data;
using TechMoveGLMS.Models;
using TechMoveGLMS.Services;
using Xunit;

namespace TechMoveGLMS.Tests.Services
{
    public class ContractServiceTests
    {
        [Fact]
        public async Task CanCreateServiceRequestAsync_ReturnsFalse_ForExpiredContract()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("Test_Expired")
                .Options;

            using var context = new ApplicationDbContext(options);
            var contract = new Contract { Id = 1, Status = "Expired" };
            context.Contracts.Add(contract);
            await context.SaveChangesAsync();

            var service = new ContractService(context);
            Assert.False(await service.CanCreateServiceRequestAsync(1));
        }

        [Fact]
        public async Task CanCreateServiceRequestAsync_ReturnsFalse_ForOnHoldContract()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("Test_OnHold")
                .Options;

            using var context = new ApplicationDbContext(options);
            var contract = new Contract { Id = 2, Status = "On Hold" };
            context.Contracts.Add(contract);
            await context.SaveChangesAsync();

            var service = new ContractService(context);
            Assert.False(await service.CanCreateServiceRequestAsync(2));
        }

        [Fact]
        public async Task CanCreateServiceRequestAsync_ReturnsTrue_ForActiveContract()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("Test_Active")
                .Options;

            using var context = new ApplicationDbContext(options);
            var contract = new Contract { Id = 3, Status = "Active" };
            context.Contracts.Add(contract);
            await context.SaveChangesAsync();

            var service = new ContractService(context);
            Assert.True(await service.CanCreateServiceRequestAsync(3));
        }
    }
}