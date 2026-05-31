using Acme.Center.Platform.Publishing.Application.QueryServices;
using Acme.Center.Platform.Publishing.Domain.Model.Aggregate;
using Acme.Center.Platform.Publishing.Domain.Model.Queries;
using Acme.Center.Platform.Publishing.Domain.Repositories;

namespace Acme.Center.Platform.Publishing.Application.Internal.QueryServices;

/// <summary>
///     Tutorial query service
/// </summary>
/// <param name="tutorialRepository">
///     Tutorial repository
/// </param>
public class TutorialQueryService(ITutorialRepository tutorialRepository) : ITutorialQueryService

{
    /// <inheritdoc />
    public async Task<Tutorial?> Handle(GetTutorialByIdQuery query, CancellationToken cancellationToken)
    {
        return await tutorialRepository.FindByIdAsync(query.TutorialId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Tutorial>> Handle(GetAllTutorialsQuery query, CancellationToken cancellationToken)
    {
        return await tutorialRepository.ListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Tutorial>> Handle(GetAllTutorialsByCategoryIdQuery query,
        CancellationToken cancellationToken)
    {
        return await tutorialRepository.FindByCategoryIdAsync(query.CategoryId, cancellationToken);
    }
}