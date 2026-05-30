using Acme.Center.Platform.Publishing.Domain.Model.Aggregate;
using Acme.Center.Platform.Publishing.Interfaces.Rest.Resources;
using Microsoft.OpenApi;


namespace Acme.Center.Platform.Publishing.Interfaces.Rest.Transform;

/// <summary>
///     Assembler class to convert Tutorial entity to TutorialResource
/// </summary>
public static class TutorialResourceFromEntityAssembler
{
    /// <summary>
    ///     Convert Tutorial entity to TutorialResource
    /// </summary>
    /// <param name="entity">
    ///     The <see cref="Tutorial" /> entity to convert
    /// </param>
    /// <returns>
    ///     The <see cref="TutorialResource" /> resource
    /// </returns>
    public static TutorialResource ToResourceFromEntity(Tutorial entity)
    {
        return new TutorialResource(
            entity.Id,
            entity.Title,
            entity.Summary,
            CategoryResourceFromEntityAssembler.ToResourceFromEntity(entity.Category),
            entity.Status.GetDisplayName());
    }
}