using System.Net.Mime;
using Acme.Center.Platform.Publishing.Application.CommandServices;
using Acme.Center.Platform.Publishing.Application.QueryServices;
using Acme.Center.Platform.Publishing.Domain.Model;
using Acme.Center.Platform.Publishing.Domain.Model.Queries;
using Acme.Center.Platform.Publishing.Interfaces.Rest.Resources;
using Acme.Center.Platform.Publishing.Interfaces.Rest.Transform;
using Acme.Center.Platform.Resources.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
// For IStringLocalizer
// For ErrorMessages resource

// For PublishingError enum

namespace Acme.Center.Platform.Publishing.Interfaces.Rest;

/// <summary>
///     The tutorial controller
/// </summary>
/// <param name="tutorialQueryService">
///     The <see cref="ITutorialQueryService" /> instance to query tutorials
/// </param>
/// <param name="tutorialCommandService">
///     The <see cref="ITutorialCommandService" /> instance to execute commands on tutorials
/// </param>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available Tutorial endpoints")]
public class TutorialsController(
    ITutorialQueryService tutorialQueryService,
    ITutorialCommandService tutorialCommandService,
    IStringLocalizer<ErrorMessages> localizer) // Inject IStringLocalizer
    : ControllerBase
{
    private readonly IStringLocalizer<ErrorMessages> _localizer = localizer;

    /// <summary>
    ///     Get a tutorial by its id
    /// </summary>
    /// <param name="tutorialId">
    ///     The tutorial id
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     The <see cref="TutorialResource" /> with the tutorial if found, otherwise it returns a response with
    ///     <see cref="NotFoundResult" />
    /// </returns>
    [HttpGet("{tutorialId:int}")]
    [SwaggerOperation(
        Summary = "Get a tutorial by its id",
        Description = "Get a tutorial by its id",
        OperationId = "GetTutorialById")]
    [SwaggerResponse(StatusCodes.Status200OK, "The tutorial was found", typeof(TutorialResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The tutorial was not found")]
    public async Task<IActionResult> GetTutorialById([FromRoute] int tutorialId, CancellationToken cancellationToken)
    {
        var tutorial = await tutorialQueryService.Handle(new GetTutorialByIdQuery(tutorialId), cancellationToken);
        if (tutorial is null)
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: _localizer[nameof(PublishingError.TutorialNotFound)],
                detail: _localizer[nameof(PublishingError.TutorialNotFound)]
            );
        var tutorialResource = TutorialResourceFromEntityAssembler.ToResourceFromEntity(tutorial);
        return Ok(tutorialResource);
    }

    /// <summary>
    ///     Create a tutorial
    /// </summary>
    /// <param name="resource">
    ///     The <see cref="CreateTutorialResource" /> with the tutorial data to create
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     The <see cref="TutorialResource" /> with the tutorial created if successful, otherwise it returns a response with
    ///     <see cref="BadRequestResult" />
    /// </returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a tutorial",
        Description = "Create a tutorial",
        OperationId = "CreateTutorial")]
    [SwaggerResponse(StatusCodes.Status201Created, "The tutorial was created", typeof(TutorialResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The tutorial was not created")]
    public async Task<IActionResult> CreateTutorial([FromBody] CreateTutorialResource resource,
        CancellationToken cancellationToken)
    {
        var createTutorialCommand = CreateTutorialCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await tutorialCommandService.Handle(createTutorialCommand, cancellationToken);
        if (result.IsFailure)
        {
            var statusCode = result.Error switch
            {
                PublishingError.CategoryNotFound => StatusCodes.Status400BadRequest,
                PublishingError.DuplicateTutorialTitle => StatusCodes.Status409Conflict,
                PublishingError.OperationCancelled => StatusCodes.Status409Conflict,
                PublishingError.DatabaseError => StatusCodes.Status500InternalServerError,
                PublishingError.InternalServerError => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status400BadRequest
            };
            return Problem(
                statusCode: statusCode,
                title: _localizer[$"{result.Error}"],
                detail: result.Message
            );
        }

        var tutorial = result.Value;
        var tutorialResource = TutorialResourceFromEntityAssembler.ToResourceFromEntity(tutorial);
        return CreatedAtAction(nameof(GetTutorialById), new { tutorialId = tutorial.Id }, tutorialResource);
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all tutorials",
        Description = "Get all tutorials",
        OperationId = "GetAllTutorials")]
    [SwaggerResponse(StatusCodes.Status200OK, "The tutorials were found", typeof(IEnumerable<TutorialResource>))]
    public async Task<IActionResult> GetAllTutorials(CancellationToken cancellationToken)
    {
        var getAllTutorialsQuery = new GetAllTutorialsQuery();
        var tutorials = await tutorialQueryService.Handle(getAllTutorialsQuery, cancellationToken);
        var tutorialResources = tutorials.Select(TutorialResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(tutorialResources);
    }

    [HttpPost("{tutorialId:int}/videos")]
    [SwaggerOperation(
        Summary = "Add a video to a tutorial",
        Description = "Add a video to a tutorial",
        OperationId = "AddVideoToTutorial")]
    [SwaggerResponse(StatusCodes.Status201Created, "The video was added to the tutorial", typeof(TutorialResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The video was not added to the tutorial")]
    public async Task<IActionResult> AddVideoToTutorial(
        [FromBody] AddVideoAssetToTutorialResource resource,
        [FromRoute] int tutorialId,
        CancellationToken cancellationToken)
    {
        var addVideoAssetToTutorialCommand =
            AddVideoAssetToTutorialCommandFromResourceAssembler.ToCommandFromResource(resource, tutorialId);
        var result = await tutorialCommandService.Handle(addVideoAssetToTutorialCommand, cancellationToken);
        if (result.IsFailure)
        {
            var statusCode = result.Error switch
            {
                PublishingError.TutorialNotFound => StatusCodes.Status404NotFound,
                PublishingError.OperationCancelled => StatusCodes.Status409Conflict,
                PublishingError.DatabaseError => StatusCodes.Status500InternalServerError,
                PublishingError.InternalServerError => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status400BadRequest
            };
            return Problem(
                statusCode: statusCode,
                title: _localizer[$"{result.Error}"],
                detail: result.Message
            );
        }

        var tutorial = result.Value;
        var tutorialResource = TutorialResourceFromEntityAssembler.ToResourceFromEntity(tutorial);
        return CreatedAtAction(nameof(GetTutorialById), new { tutorialId = tutorial.Id }, tutorialResource);
    }
}