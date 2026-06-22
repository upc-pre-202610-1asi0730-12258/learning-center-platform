using System.Net.Mime;
using Acme.Center.Platform.Publishing.Application.CommandServices;
using Acme.Center.Platform.Publishing.Application.QueryServices;
using Acme.Center.Platform.Publishing.Domain.Model.Queries;
using Acme.Center.Platform.Publishing.Interfaces.Rest.Resources;
using Acme.Center.Platform.Publishing.Interfaces.Rest.Transform;
using Acme.Center.Platform.Resources.Errors;
using Acme.Center.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Acme.Center.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
// Corrected using directive
// For ProblemDetailsFactory

// For PublishingError enum

namespace Acme.Center.Platform.Publishing.Interfaces.Rest;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available Category Endpoints.")]
public class CategoriesController(
    ICategoryCommandService categoryCommandService,
    ICategoryQueryService categoryQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer, // Renamed for clarity
    ProblemDetailsFactory problemDetailsFactory) // Inject ProblemDetailsFactory
    : ControllerBase
{
    private readonly IStringLocalizer<ErrorMessages> _errorLocalizer = errorLocalizer;
    private readonly ProblemDetailsFactory _problemDetailsFactory = problemDetailsFactory;

    [HttpGet("{categoryId:int}")]
    [SwaggerOperation(
        Summary = "Get all categories",
        Description = "Get all categories",
        OperationId = "GetAllCategories")]
    [SwaggerResponse(StatusCodes.Status200OK, "The list of categories", typeof(CategoryResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No categories found")]
    public async Task<IActionResult> GetCategoryById(int categoryId, CancellationToken cancellationToken)
    {
        var getCategoryByIdQuery = new GetCategoryByIdQuery(categoryId);
        var category = await categoryQueryService.Handle(getCategoryByIdQuery, cancellationToken);

        return PublishingActionResultAssembler.ToActionResultFromGetCategoryByIdResult(
            this,
            category,
            _errorLocalizer,
            _problemDetailsFactory,
            foundCategory => Ok(CategoryResourceFromEntityAssembler.ToResourceFromEntity(foundCategory))
        );
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new category",
        Description = "Create a new category",
        OperationId = "CreateCategory")]
    [SwaggerResponse(StatusCodes.Status201Created, "The category was created", typeof(CategoryResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The category could not be created")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryResource resource,
        CancellationToken cancellationToken)
    {
        var createCategoryCommand = CreateCategoryCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await categoryCommandService.Handle(createCategoryCommand, cancellationToken);

        return PublishingActionResultAssembler.ToActionResultFromCreateCategoryResult(
            this,
            result,
            _errorLocalizer,
            _problemDetailsFactory,
            createdCategory => CreatedAtAction(nameof(GetCategoryById), new { categoryId = createdCategory.Id },
                CategoryResourceFromEntityAssembler.ToResourceFromEntity(createdCategory))
        );
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all categories",
        Description = "Get all categories",
        OperationId = "GetAllCategories")]
    [SwaggerResponse(StatusCodes.Status200OK, "The list of categories", typeof(IEnumerable<CategoryResource>))]
    public async Task<IActionResult> GetAllCategories(CancellationToken cancellationToken)
    {
        var getAllCategoriesQuery = new GetAllCategoriesQuery();
        var categories = await categoryQueryService.Handle(getAllCategoriesQuery, cancellationToken);
        var categoryResources = categories.Select(CategoryResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(categoryResources);
    }
}