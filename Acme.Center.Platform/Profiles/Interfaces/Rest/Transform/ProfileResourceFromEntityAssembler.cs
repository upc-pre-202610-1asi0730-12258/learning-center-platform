using Acme.Center.Platform.Profiles.Domain.Model.Aggregates;
using Acme.Center.Platform.Profiles.Interfaces.Rest.Resources;

namespace Acme.Center.Platform.Profiles.Interfaces.Rest.Transform;

/// <summary>
/// Assembler class to convert Profile entity to ProfileResource 
/// </summary>
public static class ProfileResourceFromEntityAssembler
{
    /// <summary>
    /// Convert Profile entity to ProfileResource 
    /// </summary>
    /// <param name="entity">
    /// <see cref="Profile"/> entity to convert
    /// </param>
    /// <returns>
    /// <see cref="ProfileResource"/> converted from <see cref="Profile"/> entity
    /// </returns>
    public static ProfileResource ToResourceFromEntity(Profile entity)
    {
        return new ProfileResource(entity.Id, entity.FullName, entity.EmailAddress, entity.StreetAddress);
    }
}