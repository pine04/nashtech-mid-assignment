using System.Net.Mime;
using LibraryManagement.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace LibraryManagement.Middlewares
{
    public class ApplicationExceptionHandler : IExceptionHandler
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public ApplicationExceptionHandler(ProblemDetailsFactory problemDetailsFactory)
        {
            _problemDetailsFactory = problemDetailsFactory;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            (int statusCode, string title) = GetStatusCodeAndTitle(exception);
            string message = (exception is DomainException) ? exception.Message : "An error happened on the server.";

            ProblemDetails problemDetails = _problemDetailsFactory.CreateProblemDetails(
                httpContext,
                statusCode,
                title,
                detail: message
            );

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = MediaTypeNames.Application.ProblemJson;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }

        private static (int, string) GetStatusCodeAndTitle(Exception exception)
        {
            return exception switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
                NonExistentCategoryException => (StatusCodes.Status400BadRequest, "Bad Request"),
                NonExistentUserException => (StatusCodes.Status400BadRequest, "Bad Request"),
                NonExistentBookException => (StatusCodes.Status400BadRequest, "Bad Request"),
                InvalidSubClaimException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
                MonthlyLimitReachedException => (StatusCodes.Status429TooManyRequests, "Too Many Requests"),
                NoBooksProvidedException => (StatusCodes.Status400BadRequest, "Bad Request"),
                TooManyBooksException => (StatusCodes.Status400BadRequest, "Bad Request"),
                BookNotAvailableException => (StatusCodes.Status409Conflict, "Conflict"),
                ForbiddenException => (StatusCodes.Status403Forbidden, "Forbidden"),
                EmailAlreadyUsedException => (StatusCodes.Status409Conflict, "Conflict"),
                UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
                RequestAlreadySettledException => (StatusCodes.Status409Conflict, "Conflict"),
                InvalidBookQuantityException => (StatusCodes.Status409Conflict, "Conflict"),
                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
            };
        }
    }
}