using TechMoveGLMS.Services;
using Xunit;

namespace TechMoveGLMS.Tests.Services
{
    public class CurrencyServiceTests
    {
        [Fact]
        public async Task ConvertUsdToZarAsync_ReturnsPositiveValue()
        {
            var service = new CurrencyService(new HttpClient());
            var result = await service.ConvertUsdToZarAsync(100);

            Assert.True(result > 0, "Should return a positive ZAR amount");
        }

        [Fact]
        public async Task ConvertUsdToZarAsync_ReturnsZero_ForInvalidAmount()
        {
            var service = new CurrencyService(new HttpClient());

            Assert.Equal(0m, await service.ConvertUsdToZarAsync(0));
            Assert.Equal(0m, await service.ConvertUsdToZarAsync(-100));
        }
    }
}