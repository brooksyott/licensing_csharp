using Microsoft.EntityFrameworkCore;

namespace Licensing.Common
{
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
                            return new ServiceResult<T>() { ErrorMessage = new ErrorMessageStruct(postgresExInner.Message), Status = ResultStatusCode.Conflict };
                        }

                        return new ServiceResult<T>() { ErrorMessage = new ErrorMessageStruct(postgresExInner.Message), Status = ResultStatusCode.BadRequest };
                    }
                    else
                    {
                        return new ServiceResult<T>() { ErrorMessage = new ErrorMessageStruct(dbUpdateEx.InnerException?.GetType().Name), Status = ResultStatusCode.InternalServerError };
                    }

                case Npgsql.PostgresException postgresEx:
                    return new ServiceResult<T>() { ErrorMessage = new ErrorMessageStruct(postgresEx.Message), Status = ResultStatusCode.InternalServerError };

                case InvalidOperationException invalidOpEx:
                    return new ServiceResult<T>() { ErrorMessage = new ErrorMessageStruct(invalidOpEx.Message), Status = ResultStatusCode.BadRequest };

                default:
                    return new ServiceResult<T>() { ErrorMessage = new ErrorMessageStruct(ex.Message), Status = ResultStatusCode.InternalServerError };
            }
        }
    }
}
