using CashFlow.Consolidated.Domain.SharedKernel.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CashFlow.Consolidated.Api.Filters
{
    public class GlobalExceptionFilter(IHostEnvironment hostEnvironment) : IExceptionFilter
    {
        private readonly IHostEnvironment _hostEnvironment = hostEnvironment;

        public void OnException(ExceptionContext context)
        {
            var details = new ProblemDetails();
            var exception = context.Exception;

            if (_hostEnvironment.IsDevelopment())
                details.Extensions.Add("StackTrace", exception.StackTrace);

            if (exception is UnprocessableContentException)
            {
                details.Title = "Unprocessable Content";
                details.Status = StatusCodes.Status422UnprocessableEntity;
                details.Type = "UnprocessableEntity";
                details.Detail = exception!.Message;
            }
            else
            {
                details.Title = "An unexpected error ocurred";
                details.Status = StatusCodes.Status500InternalServerError;
                details.Type = "UnexpectedError";
                details.Detail = exception.Message;
            }

            context.HttpContext.Response.StatusCode = (int)details.Status;
            context.Result = new ObjectResult(details);
            context.ExceptionHandled = true;
        }
    }
}
