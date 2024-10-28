using ACME.LearningCenterPlatform.API.Publishing.Domain.Model.Aggregate;
using ACME.LearningCenterPlatform.API.Publishing.Interfaces.REST.Resources;
using Microsoft.OpenApi.Extensions;

namespace ACME.LearningCenterPlatform.API.Publishing.Interfaces.REST.Transform;

/// <summary>
/// Assembler class to convert Tutorial entity to TutorialResource 
/// </summary>
public static class TutorialResourceFromEntityAssembler
{
    /// <summary>
    /// Convert Tutorial entity to TutorialResource 
    /// </summary>
    /// <param name="entity">
    /// The <see cref="Tutorial"/> entity to convert
    /// </param>
    /// <returns>
    /// The <see cref="TutorialResource"/> resource
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