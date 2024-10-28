namespace ACME.LearningCenterPlatform.API.Publishing.Interfaces.REST.Resources;

/// <summary>
/// Create tutorial resource for REST API 
/// </summary>
/// <param name="Title">
/// The title of the tutorial
/// </param>
/// <param name="Summary">
/// The summary of the tutorial
/// </param>
/// <param name="CategoryId">
/// The unique identifier of the category
/// </param>
public record CreateTutorialResource(string Title, string Summary, int CategoryId);