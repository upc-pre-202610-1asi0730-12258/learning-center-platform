using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure; // For base ProblemDetailsFactory
using Microsoft.Extensions.Localization;
using Acme.Center.Platform.Resources.Errors; // For ErrorMessages
using Acme.Center.Platform.Resources.Shared; // For Shared.Commons
using System;
using System.Net.Mime; // For StatusCodes

namespace Acme.Center.Platform.Shared.Interfaces.Rest.ProblemDetails
{
    public class ProblemDetailsFactory
    {
        private readonly IStringLocalizer<ErrorMessages> _errorLocalizer;
        private readonly IStringLocalizer<Commons> _commonLocalizer; // Corrected to Commons
        private readonly Microsoft.AspNetCore.Mvc.Infrastructure.ProblemDetailsFactory _aspNetCoreProblemDetailsFactory; // Corrected type and name

        public ProblemDetailsFactory(
            IStringLocalizer<ErrorMessages> errorLocalizer,
            IStringLocalizer<Commons> commonLocalizer, // Corrected to Commons
            Microsoft.AspNetCore.Mvc.Infrastructure.ProblemDetailsFactory aspNetCoreProblemDetailsFactory) // Corrected injected type
        {
            _errorLocalizer = errorLocalizer;
            _commonLocalizer = commonLocalizer;
            _aspNetCoreProblemDetailsFactory = aspNetCoreProblemDetailsFactory; // Corrected assignment
        }

        public IActionResult CreateProblemDetails(
            ControllerBase controller,
            int statusCode,
            Enum? errorEnum, // The specific error enum (IamError, ProfilesError, etc.)
            string detailMessage) // The localized message from the application service
        {
            // Leverage the base ProblemDetailsFactory for initial creation
            var problemDetails = _aspNetCoreProblemDetailsFactory.CreateProblemDetails( // Corrected usage
                controller.HttpContext,
                statusCode,
                title: errorEnum != null ? _errorLocalizer[$"{errorEnum}"] : _commonLocalizer["GenericError"],
                detail: detailMessage
            );

            // Ensure problemDetails is not null (shouldn't be with default factory)
            if (problemDetails == null)
            {
                problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
                {
                    Status = statusCode,
                    Title = errorEnum != null ? _errorLocalizer[$"{errorEnum}"] : _commonLocalizer["GenericError"],
                    Detail = detailMessage,
                    Instance = controller.HttpContext.Request.Path
                };
            }
            else
            {
                problemDetails.Title = errorEnum != null ? _errorLocalizer[$"{errorEnum}"] : _commonLocalizer["GenericError"];
                problemDetails.Detail = detailMessage;
                problemDetails.Instance = controller.HttpContext.Request.Path;
            }

            return controller.StatusCode(statusCode, problemDetails);
        }

        // Overload for when there's no specific error enum, just a generic message
        public IActionResult CreateProblemDetails(
            ControllerBase controller,
            int statusCode,
            string titleKey, // Key for localized title
            string detailKey, // Key for localized detail
            params object[] detailArgs)
        {
            var problemDetails = _aspNetCoreProblemDetailsFactory.CreateProblemDetails( // Corrected usage
                controller.HttpContext,
                statusCode,
                title: _commonLocalizer[titleKey],
                detail: _errorLocalizer[detailKey, detailArgs],
                instance: controller.HttpContext.Request.Path
            );
            return controller.StatusCode(statusCode, problemDetails);
        }
    }
}