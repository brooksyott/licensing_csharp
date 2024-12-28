using Licensing.Data;
using Licensing.Keys;
using Microsoft.EntityFrameworkCore;

namespace Licensing.Common
{
    /// <summary>
    /// Base service class for common service functionality
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseService<T> where T : class
    {
        protected readonly ILogger<T> _logger;
        protected readonly LicensingContext _dbContext;

        public BaseService(ILogger<T> logger, LicensingContext context)
        {
            _logger = logger;
            _dbContext = context;
        }

        /// <summary>
        /// Returns a service result based on the specified exception
        /// </summary>
        /// <param name="ex"></param>
        protected ServiceResult<S> ReturnException<S>(Exception ex, string logMessage = "")
        {
            if (String.IsNullOrWhiteSpace(logMessage))
                _logger.LogError($"{logMessage}");
            else
                _logger.LogError($"{logMessage}: {ex.Message}");

            return ExceptionHandler.ReturnException<S>(ex);
        }
    }
}
