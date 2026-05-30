using Acme.Center.Platform.Publishing.Application.CommandServices;
using Acme.Center.Platform.Publishing.Domain.Model;
using Acme.Center.Platform.Publishing.Domain.Model.Aggregate;
using Acme.Center.Platform.Publishing.Domain.Model.Commands;
using Acme.Center.Platform.Publishing.Domain.Repositories;
using Acme.Center.Platform.Shared.Application.Model;
using Acme.Center.Platform.Shared.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Acme.Center.Platform.Publishing.Application.Internal.CommandServices;

public class TutorialCommandService(
    ITutorialRepository tutorialRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork)
    : ITutorialCommandService
{
    /// <inheritdoc />
    public async Task<Result<Tutorial>> Handle(AddVideoAssetToTutorialCommand command, CancellationToken cancellationToken)
    {
        var tutorial = await tutorialRepository.FindByIdAsync(command.TutorialId, cancellationToken);
        if (tutorial is null) return Result<Tutorial>.Failure(PublishingErrors.TutorialNotFound);
        tutorial.AddVideo(command.VideoUrl);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<Tutorial>.Success(tutorial);
    }

    /// <inheritdoc />
    public async Task<Result<Tutorial>> Handle(CreateTutorialCommand command, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.FindByIdAsync(command.CategoryId, cancellationToken);
        if (category is null) return Result<Tutorial>.Failure(PublishingErrors.CategoryNotFound);
        if (await tutorialRepository.ExistsByTitleAsync(command.Title, cancellationToken))
            return Result<Tutorial>.Failure(PublishingErrors.DuplicateTutorialTitle);
        var tutorial = new Tutorial(command);
        await tutorialRepository.AddAsync(tutorial, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        tutorial.Category = category;
        return Result<Tutorial>.Success(tutorial);
    }
}