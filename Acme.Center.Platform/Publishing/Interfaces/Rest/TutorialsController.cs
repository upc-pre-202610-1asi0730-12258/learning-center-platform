using System.Net.Mime;
using Acme.Center.Platform.Publishing.Application.CommandServices;
using Acme.Center.Platform.Publishing.Application.QueryServices;
using Acme.Center.Platform.Publishing.Domain.Model.Queries;
using Acme.Center.Platform.Publishing.Interfaces.Rest.Resources;
using Acme.Center.Platform.Publishing.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
    ITutorialCommandService tutorialCommandService) : ControllerBase
{
    /// <summary>
    ///     Get a tutorial by its id
    /// </summary>
    /// <param name="tutorialId">
    ///     The tutorial id
    /// </param>
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
    public async Task<IActionResult> GetTutorialById([FromRoute] int tutorialId)
    {
        var tutorial = await tutorialQueryService.Handle(new GetTutorialByIdQuery(tutorialId));
        if (tutorial is null) return NotFound();
        var tutorialResource = TutorialResourceFromEntityAssembler.ToResourceFromEntity(tutorial);
        return Ok(tutorialResource);
    }

    /// <summary>
    ///     Create a tutorial
    /// </summary>
    /// <param name="resource">
    ///     The <see cref="CreateTutorialResource" /> with the tutorial data to create
    /// </param>
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
    public async Task<IActionResult> CreateTutorial([FromBody] CreateTutorialResource resource)
    {
        var createTutorialCommand = CreateTutorialCommandFromResourceAssembler.ToCommandFromResource(resource);
        var tutorial = await tutorialCommandService.Handle(createTutorialCommand);
        if (tutorial is null) return BadRequest();
        var tutorialResource = TutorialResourceFromEntityAssembler.ToResourceFromEntity(tutorial);
        return CreatedAtAction(nameof(GetTutorialById), new { tutorialId = tutorial.Id }, tutorialResource);
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all tutorials",
        Description = "Get all tutorials",
        OperationId = "GetAllTutorials")]
    [SwaggerResponse(StatusCodes.Status200OK, "The tutorials were found", typeof(IEnumerable<TutorialResource>))]
    public async Task<IActionResult> GetAllTutorials()
    {
        var getAllTutorialsQuery = new GetAllTutorialsQuery();
        var tutorials = await tutorialQueryService.Handle(getAllTutorialsQuery);
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
        [FromRoute] int tutorialId)
    {
        var addVideoAssetToTutorialCommand =
            AddVideoAssetToTutorialCommandFromResourceAssembler.ToCommandFromResource(resource, tutorialId);
        var tutorial = await tutorialCommandService.Handle(addVideoAssetToTutorialCommand);
        if (tutorial is null) return BadRequest();
        var tutorialResource = TutorialResourceFromEntityAssembler.ToResourceFromEntity(tutorial);
        return CreatedAtAction(nameof(GetTutorialById), new { tutorialId = tutorial.Id }, tutorialResource);
    }
}