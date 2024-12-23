using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Licensing.Common
{
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

    public static class HttpStatusExtensions
    {
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

    public class ErrorMessageStruct
    {
        public ErrorMessageStruct(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }

    public class ServiceResult<T>
    {

        public ErrorMessageStruct? ErrorMessage { get; set; }
        public ResultStatusCode Status { get; set; }
        public T? Data { get; set; }

        public static ServiceResult<T> Success(T data) => new ServiceResult<T> { Status = ResultStatusCode.Success, Data = data };
        public static ServiceResult<T> Failure(ResultStatusCode status, string errorMessage) => new ServiceResult<T> { Status = status, ErrorMessage = new ErrorMessageStruct(errorMessage) };

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
