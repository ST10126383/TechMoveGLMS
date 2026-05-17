namespace TechMoveGLMS.Services
{

        public interface ICurrencyService
        {
            Task<decimal> ConvertUsdToZarAsync(decimal usdAmount);
        }
    }

