using Microsoft.EntityFrameworkCore;

namespace Licensing.Common
{
    /// <summary>
    /// Exception handler for the service layer.
    /// Provides a consistent way to handle exceptions and return appropriate error messages.
    /// </summary>
    public static class ExceptionHandler
    {
        public static ServiceResult<T> ReturnException<T>(Exception ex, string logMessage = "") 
        {
            switch (ex)
            {
                case DbUpdateException dbUpdateEx:
                    if (dbUpdateEx.InnerException is Npgsql.PostgresException postgresExInner)
                    {
                        var errorMessage = postgresExInner.Message;
                        if (errorMessage.Contains("duplicate"))
                        {
                            return new ServiceResult<T>() { ErrorMessage = new ErrorInformation(postgresExInner.Message), Status = ResultStatusCode.Conflict };
                        }
                        if (errorMessage.Contains("foreign"))
                        {
                            return new ServiceResult<T>() { ErrorMessage = new ErrorInformation("Foreign key exception, please check customer exists and/or other constraints"), Status = ResultStatusCode.BadRequest };
                        }

                        return new ServiceResult<T>() { ErrorMessage = new ErrorInformation(postgresExInner.Message), Status = ResultStatusCode.BadRequest };
                    }
                    else
                    {
                        return new ServiceResult<T>() { ErrorMessage = new ErrorInformation(dbUpdateEx.InnerException?.GetType().Name), Status = ResultStatusCode.InternalServerError };
                    }

                case Npgsql.PostgresException postgresEx:
                    return new ServiceResult<T>() { ErrorMessage = new ErrorInformation(postgresEx.Message), Status = ResultStatusCode.InternalServerError };

                case InvalidOperationException invalidOpEx:
                    return new ServiceResult<T>() { ErrorMessage = new ErrorInformation(invalidOpEx.Message), Status = ResultStatusCode.BadRequest };

                default:
                    return new ServiceResult<T>() { ErrorMessage = new ErrorInformation(ex.Message), Status = ResultStatusCode.InternalServerError };
            }
        }
    }
}
