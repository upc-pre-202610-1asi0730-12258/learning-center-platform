using Acme.Center.Platform.Publishing.Domain.Model.Aggregate;
using Acme.Center.Platform.Shared.Domain.Repositories;

namespace Acme.Center.Platform.Publishing.Domain.Repositories;

/// <summary>
///     Represents the tutorial repository in the ACME Learning Center Platform.
/// </summary>
public interface ITutorialRepository : IBaseRepository<Tutorial>
{
    /// <summary>
    ///     Finds a tutorial by category id asynchronously.
    /// </summary>
    /// <param name="categoryId">
    ///     The id of the category to find tutorials by.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A collection of tutorials that belong to the category.
    /// </returns>
    Task<IEnumerable<Tutorial>> FindByCategoryIdAsync(int categoryId, CancellationToken cancellationToken);

    Task<bool> ExistsByTitleAsync(string title, CancellationToken cancellationToken);
}