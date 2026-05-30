using Acme.Center.Platform.Profiles.Domain.Model.Commands;
using Acme.Center.Platform.Profiles.Interfaces.Rest.Resources;

namespace Acme.Center.Platform.Profiles.Interfaces.Rest.Transform;

/// <summary>
/// Assembler to create a CreateProfileCommand command from a resource 
/// </summary>
public static class CreateProfileCommandFromResourceAssembler
{
    /// <summary>
    /// Create a CreateProfileCommand command from a resource 
    /// </summary>
    /// <param name="resource">
    /// The <see cref="CreateProfileResource"/> resource to create the command from
    /// </param>
    /// <returns>
    /// The <see cref="CreateProfileCommand"/> command created from the resource
    /// </returns>
    public static CreateProfileCommand ToCommandFromResource(CreateProfileResource resource)
    {
        return new CreateProfileCommand(
            resource.FirstName,
            resource.LastName,
            resource.Email,
            resource.Street,
            resource.Number,
            resource.City,
            resource.PostalCode,
            resource.Country
        );
    }
}