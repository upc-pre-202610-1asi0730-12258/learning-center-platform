using Acme.Center.Platform.Publishing.Domain.Model.Entities;
using Acme.Center.Platform.Publishing.Interfaces.Rest.Resources;

namespace Acme.Center.Platform.Publishing.Interfaces.Rest.Transform;

/// <summary>
///     Assembler class to convert Category to CategoryResource
/// </summary>
public static class CategoryResourceFromEntityAssembler
{
    /// <summary>
    ///     Convert Category to CategoryResource
    /// </summary>
    /// <param name="entity">
    ///     The <see cref="Category" /> entity
    /// </param>
    /// <returns>
    ///     The <see cref="CategoryResource" /> resource
    /// </returns>
    public static CategoryResource ToResourceFromEntity(Category entity)
    {
        return new CategoryResource(entity.Id, entity.Name);
    }
}