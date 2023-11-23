// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using System.Net;

namespace RebacExperiments.Server.Api.Infrastructure.Errors
{
    /// <summary>
    /// Options for the <see cref="ApplicationErrorHandler"/>.
    /// </summary>
    public class ApplicationErrorHandlerOptions
    {
        /// <summary>
        /// Gets or sets the option to include the Exception Details in the response.
        /// </summary>
        public bool IncludeExceptionDetails { get; set; } = false;
    }

    /// <summary>
    /// Handles errors returned by the application.
    /// </summary>
    public class ApplicationErrorHandler
    {
        private readonly ILogger<ApplicationErrorHandler> _logger;
        
        private readonly ApplicationErrorHandlerOptions _options;

        public ApplicationErrorHandler(ILogger<ApplicationErrorHandler> logger, IOptions<ApplicationErrorHandlerOptions> options) 
        { 
            _logger = logger;
            _options = options.Value;
        }

        public ObjectResult HandleInvalidModelState(HttpContext httpContext, ModelStateDictionary modelStateDictionary)
        {
            _logger.TraceMethodEntry();

            var details = new ValidationProblemDetails(modelStateDictionary)
            {
                Title = "Validation Failed",
                Type = "ValidationError",
                Status = (int)HttpStatusCode.BadRequest,
                Instance = httpContext.Request.Path,
            };

            details.Extensions.Add("error-code", ErrorCodes.ValidationFailed);
            details.Extensions.Add("trace-id", httpContext.TraceIdentifier);

            return new ObjectResult(details)
            {
                ContentTypes = { "application/problem+json" },
                StatusCode = (int)HttpStatusCode.BadRequest,
            };
        }

        public ObjectResult HandleException(HttpContext httpContext, Exception exception)
        {
            _logger.TraceMethodEntry();

            _logger.LogError(exception, "Call to '{RequestPath}' failed due to an Exception", httpContext.Request.Path);

            return exception switch
            {
                AuthenticationFailedException e => HandleAuthenticationException(httpContext, e),
                EntityConcurrencyException e => HandleEntityConcurrencyException(httpContext, e),
                EntityNotFoundException e => HandleEntityNotFoundException(httpContext, e),
                EntityUnauthorizedAccessException e => HandleEntityUnauthorizedException(httpContext, e),
                Exception e => HandleSystemException(httpContext, e)
            };
         }

        private ObjectResult HandleAuthenticationException(HttpContext httpContext, AuthenticationFailedException e)
        {
            _logger.TraceMethodEntry();

            var details = new ProblemDetails
            {
                Title = e.ErrorMessage,
                Type = nameof(AuthenticationFailedException),
                Status = (int)HttpStatusCode.Unauthorized,
                Instance = httpContext.Request.Path,
            };

            details.Extensions.Add("error-code", ErrorCodes.AuthenticationFailed);
            details.Extensions.Add("trace-id", httpContext.TraceIdentifier);

            AddExceptionDetails(details, e);

            return new ObjectResult(details)
            {
                ContentTypes = { "application/problem+json" },
                StatusCode = (int)HttpStatusCode.Unauthorized,
            };
        }

        private ObjectResult HandleEntityConcurrencyException(HttpContext httpContext, EntityConcurrencyException e)
        {
            _logger.TraceMethodEntry();

            var details = new ProblemDetails
            {
                Title = e.ErrorMessage,
                Type = nameof(EntityConcurrencyException),
                Status = (int)HttpStatusCode.Unauthorized,
                Instance = httpContext.Request.Path,
            };

            details.Extensions.Add("error-code", ErrorCodes.EntityConcurrencyFailure);
            details.Extensions.Add("trace-id", httpContext.TraceIdentifier);

            AddExceptionDetails(details, e);

            return new ObjectResult(details)
            {
                ContentTypes = { "application/problem+json" },
                StatusCode = (int)HttpStatusCode.BadRequest,
            };
        }

        private ObjectResult HandleEntityNotFoundException(HttpContext httpContext, EntityNotFoundException e)
        {
            _logger.TraceMethodEntry();

            var details = new ProblemDetails
            {
                Title = e.ErrorMessage,
                Type = nameof(EntityNotFoundException),
                Status = (int)HttpStatusCode.NotFound,
                Instance = httpContext.Request.Path,
            };

            details.Extensions.Add("error-code", ErrorCodes.EntityNotFound);
            details.Extensions.Add("trace-id", httpContext.TraceIdentifier);

            AddExceptionDetails(details, e);

            return new ObjectResult(details)
            {
                ContentTypes = { "application/problem+json" },
                StatusCode = (int)HttpStatusCode.NotFound,
            };
        }

        private ObjectResult HandleEntityUnauthorizedException(HttpContext httpContext, EntityUnauthorizedAccessException e)
        {
            _logger.TraceMethodEntry();

            var details = new ProblemDetails
            {
                Title = e.ErrorMessage,
                Type = nameof(EntityUnauthorizedAccessException),
                Status = (int)HttpStatusCode.Forbidden,
                Instance = httpContext.Request.Path,
            };

            details.Extensions.Add("error-code", ErrorCodes.EntityUnauthorized);
            details.Extensions.Add("trace-id", httpContext.TraceIdentifier);
            
            AddExceptionDetails(details, e);

            return new ObjectResult(details)
            {
                ContentTypes = { "application/problem+json" },
                StatusCode = (int)HttpStatusCode.Forbidden,
            };
        }

        private ObjectResult HandleSystemException(HttpContext httpContext, Exception e)
        {
            _logger.TraceMethodEntry();

            var details = new ProblemDetails
            {
                Title = "An Internal Server Error occured",
                Status = (int)HttpStatusCode.InternalServerError,
                Instance = httpContext.Request.Path,
            };

            details.Extensions.Add("error-code", ErrorCodes.InternalServerError);
            details.Extensions.Add("trace-id", httpContext.TraceIdentifier);

            AddExceptionDetails(details, e);

            return new ObjectResult(details)
            {
                ContentTypes = { "application/problem+json" },
                StatusCode = (int)HttpStatusCode.InternalServerError,
            };
        }

        private void AddExceptionDetails(ProblemDetails details, Exception e)
        {
            _logger.TraceMethodEntry();

            if(_options.IncludeExceptionDetails)
            {
                details.Extensions.Add("exception", e.ToString());
            }
        }
    }
}
