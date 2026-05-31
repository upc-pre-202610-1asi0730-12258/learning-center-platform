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
///     The category controller
/// </summary>
/// <param name="categoryCommandService">
///     The <see cref="ICategoryCommandService" /> category command service
/// </param>
/// <param name="categoryQueryService">
///     The <see cref="ICategoryQueryService" /> category query service
/// </param>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available Category Endpoints")]
public class CategoriesController(
    ICategoryCommandService categoryCommandService,
    ICategoryQueryService categoryQueryService,
    IStringLocalizer<ErrorMessages> localizer) // Inject IStringLocalizer
    : ControllerBase
{
    private readonly IStringLocalizer<ErrorMessages> _localizer = localizer;

    /// <summary>
    ///     Get category by id
    /// </summary>
    /// <param name="categoryId">
    ///     The category id
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     The <see cref="CategoryResource" /> category
    /// </returns>
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
        if (category is null)
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: _localizer[nameof(PublishingError.CategoryNotFound)],
                detail: _localizer[nameof(PublishingError.CategoryNotFound)]
            );
        var resource = CategoryResourceFromEntityAssembler.ToResourceFromEntity(category);
        return Ok(resource);
    }

    /// <summary>
    ///     Create a new category
    /// </summary>
    /// <param name="resource">
    ///     The <see cref="CreateCategoryResource" /> category resource
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     The <see cref="CategoryResource" /> category created, or a bad request if the category could not be created
    /// </returns>
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
        if (result.IsFailure)
        {
            var statusCode = result.Error switch
            {
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

        var category = result.Value;
        var categoryResource = CategoryResourceFromEntityAssembler.ToResourceFromEntity(category);
        return CreatedAtAction(nameof(GetCategoryById), new { categoryId = category.Id }, categoryResource);
    }

    /// <summary>
    ///     Get all categories
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     The list of <see cref="CategoryResource" /> categories
    /// </returns>
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