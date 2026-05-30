using Acme.Center.Platform.Publishing.Domain.Model.Commands;
using Acme.Center.Platform.Publishing.Interfaces.Rest.Resources;

namespace Acme.Center.Platform.Publishing.Interfaces.Rest.Transform;

/// <summary>
///     Assembler class to convert AddVideoAssetToTutorialResource to AddVideoAssetToTutorialCommand
/// </summary>
public static class AddVideoAssetToTutorialCommandFromResourceAssembler
{
    /// <summary>
    ///     Convert AddVideoAssetToTutorialResource to AddVideoAssetToTutorialCommand
    /// </summary>
    /// <param name="resource">
    ///     The <see cref="AddVideoAssetToTutorialResource" /> resource
    /// </param>
    /// <param name="TutorialId">
    ///     The tutorial id to add the video asset to
    /// </param>
    /// <returns>
    ///     The <see cref="AddVideoAssetToTutorialCommand" /> command
    /// </returns>
    public static AddVideoAssetToTutorialCommand ToCommandFromResource(
        AddVideoAssetToTutorialResource resource, int tutorialId)
    {
        return new AddVideoAssetToTutorialCommand(resource.VideoUrl, tutorialId);
    }
}