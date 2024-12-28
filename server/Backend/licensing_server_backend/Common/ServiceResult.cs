using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Licensing.Common
{
    /// <summary>
    /// Enum for Result Status Codes to provide a consistent response from services
    /// </summary>
    public enum ResultStatusCode
    {
        Success = 200,       // OK
        Created = 201,       // Resource Created
        Accepted = 202,       // Resource Created
        BadRequest = 400,    // Client Error
        Unauthorized = 401,  // Authentication Required
        Forbidden = 403,     // Authorization Failed
        NotFound = 404,      // Resource Not Found
        Conflict = 409,      // Conflict in Request
        InternalServerError = 500 // Server Error
    }

    /// <summary>
    /// Extension methods for converting ResultStatusCode to HttpStatusCode
    /// </summary>
    public static class HttpStatusExtensions
    {
        /// <summary>
        /// An abstraction to convert ResultStatusCode returned from a service to an HttpStatusCode for the response
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static HttpStatusCode EnumToStatusCode(ResultStatusCode status)
        {
            return status switch
            {
                ResultStatusCode.Success => HttpStatusCode.OK,
                ResultStatusCode.Created => HttpStatusCode.Created,
                ResultStatusCode.Accepted => HttpStatusCode.Accepted,
                ResultStatusCode.BadRequest => HttpStatusCode.BadRequest,
                ResultStatusCode.Unauthorized => HttpStatusCode.Unauthorized,
                ResultStatusCode.Forbidden => HttpStatusCode.Forbidden,
                ResultStatusCode.NotFound => HttpStatusCode.NotFound,
                ResultStatusCode.Conflict => HttpStatusCode.Conflict,
                ResultStatusCode.InternalServerError => HttpStatusCode.InternalServerError,
                _ => HttpStatusCode.InternalServerError // Default to Internal Server Error for undefined cases
            };
        }
    }

    /// <summary>
    /// Error Information class to provide a consistent error message format
    /// </summary>
    public class ErrorInformation
    {
        public ErrorInformation(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }

    /// <summary>
    /// Generic class for providing consistent responses from services
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceResult<T>
    {

        public ErrorInformation? ErrorMessage { get; set; }
        public ResultStatusCode Status { get; set; }
        public T? Data { get; set; }

        public static ServiceResult<T> Success(T data) => new ServiceResult<T> { Status = ResultStatusCode.Success, Data = data };
        public static ServiceResult<T> Failure(ResultStatusCode status, string errorMessage) => new ServiceResult<T> { Status = status, ErrorMessage = new ErrorInformation(errorMessage) };

        public IActionResult ToActionResult()
        {
            return Status switch
            {
                ResultStatusCode.Success => new OkObjectResult(Data),
                ResultStatusCode.Created => new CreatedResult("", Data),
                ResultStatusCode.BadRequest => new BadRequestObjectResult(ErrorMessage),
                ResultStatusCode.Unauthorized => new UnauthorizedResult(),
                ResultStatusCode.Forbidden => new ForbidResult(),
                ResultStatusCode.NotFound => new NotFoundObjectResult(ErrorMessage),
                ResultStatusCode.Conflict => new ConflictObjectResult(ErrorMessage),
                ResultStatusCode.InternalServerError => new ObjectResult(ErrorMessage) { StatusCode = (int)HttpStatusCode.InternalServerError },
                _ => new ObjectResult(ErrorMessage) { StatusCode = 500 }
            };
        }

    }
}
