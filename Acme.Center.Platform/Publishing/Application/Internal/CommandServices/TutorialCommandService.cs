using Acme.Center.Platform.Publishing.Application.CommandServices;
using Acme.Center.Platform.Publishing.Domain.Model;
using Acme.Center.Platform.Publishing.Domain.Model.Aggregate;
using Acme.Center.Platform.Publishing.Domain.Model.Commands;
using Acme.Center.Platform.Publishing.Domain.Repositories;
using Acme.Center.Platform.Resources.Errors;
using Acme.Center.Platform.Shared.Application.Model;
using Acme.Center.Platform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
// For PublishingError enum
// For IStringLocalizer
// For ErrorMessages resource
// For DbUpdateException

namespace Acme.Center.Platform.Publishing.Application.Internal.CommandServices;

public class TutorialCommandService(
    ITutorialRepository tutorialRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer) // Inject IStringLocalizer
    : ITutorialCommandService
{
    private readonly IStringLocalizer<ErrorMessages> _localizer = localizer;

    /// <inheritdoc />
    public async Task<Result<Tutorial>> Handle(AddVideoAssetToTutorialCommand command,
        CancellationToken cancellationToken)
    {
        var tutorial = await tutorialRepository.FindByIdAsync(command.TutorialId, cancellationToken);
        if (tutorial is null)
            return Result<Tutorial>.Failure(PublishingError.TutorialNotFound,
                _localizer[nameof(PublishingError.TutorialNotFound)]);
        tutorial.AddVideo(command.VideoUrl);
        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Tutorial>.Success(tutorial);
        }
        catch (OperationCanceledException)
        {
            return Result<Tutorial>.Failure(PublishingError.OperationCancelled,
                _localizer[nameof(PublishingError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            // Log the exception details here if an ILogger is injected
            return Result<Tutorial>.Failure(PublishingError.DatabaseError,
                _localizer[nameof(PublishingError.DatabaseError)]);
        }
        catch (Exception)
        {
            // Log the exception details here if an ILogger is injected
            return Result<Tutorial>.Failure(PublishingError.InternalServerError,
                _localizer[nameof(PublishingError.InternalServerError)]);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Tutorial>> Handle(CreateTutorialCommand command, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.FindByIdAsync(command.CategoryId, cancellationToken);
        if (category is null)
            return Result<Tutorial>.Failure(PublishingError.CategoryNotFound,
                _localizer[nameof(PublishingError.CategoryNotFound)]);
        if (await tutorialRepository.ExistsByTitleAsync(command.Title, cancellationToken))
            return Result<Tutorial>.Failure(PublishingError.DuplicateTutorialTitle,
                _localizer[nameof(PublishingError.DuplicateTutorialTitle), command.Title]);
        var tutorial = new Tutorial(command);
        try
        {
            await tutorialRepository.AddAsync(tutorial, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            tutorial.Category = category;
            return Result<Tutorial>.Success(tutorial);
        }
        catch (OperationCanceledException)
        {
            return Result<Tutorial>.Failure(PublishingError.OperationCancelled,
                _localizer[nameof(PublishingError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            // Log the exception details here if an ILogger is injected
            return Result<Tutorial>.Failure(PublishingError.DatabaseError,
                _localizer[nameof(PublishingError.DatabaseError)]);
        }
        catch (Exception)
        {
            // Log the exception details here if an ILogger is injected
            return Result<Tutorial>.Failure(PublishingError.InternalServerError,
                _localizer[nameof(PublishingError.InternalServerError)]);
        }
    }
}