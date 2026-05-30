using Acme.Center.Platform.Publishing.Application.CommandServices;
using Acme.Center.Platform.Publishing.Domain.Model;
using Acme.Center.Platform.Publishing.Domain.Model.Commands;
using Acme.Center.Platform.Publishing.Domain.Model.Entities;
using Acme.Center.Platform.Publishing.Domain.Model.Events;
using Acme.Center.Platform.Publishing.Domain.Repositories;
using Acme.Center.Platform.Shared.Application.Model;
using Acme.Center.Platform.Shared.Domain.Repositories;
using Cortex.Mediator;
using System.Threading;
using System.Threading.Tasks;

namespace Acme.Center.Platform.Publishing.Application.Internal.CommandServices;

/// <summary>
///     Represents the category command service in the ACME Learning Center Platform.
/// </summary>
/// <param name="categoryRepository">
///     The <see cref="ICategoryRepository" /> to use.
/// </param>
/// <param name="unitOfWork">
///     The <see cref="IUnitOfWork" /> to use.
/// </param>
/// <param name="domainEventPublisher">
///     The <see cref="IMediator" /> to use for publishing domain events.
/// </param>
public class CategoryCommandService(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IMediator domainEventPublisher)
    : ICategoryCommandService
{
    /// <inheritdoc />
    public async Task<Result<Category>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = new Category(command);
        try
        {
            await categoryRepository.AddAsync(category, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);

            // Publish the domain event after the category is created
            await domainEventPublisher.PublishAsync(new CategoryCreatedEvent(category.Name), cancellationToken);

            // Return the created category
            return Result<Category>.Success(category);
        }
        catch (Exception)
        {
            return Result<Category>.Failure(PublishingErrors.CategoryCreationFailed);
        }
    }
}