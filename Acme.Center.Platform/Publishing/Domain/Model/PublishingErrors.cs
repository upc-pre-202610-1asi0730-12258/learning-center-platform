using Acme.Center.Platform.Shared.Domain.Model;

namespace Acme.Center.Platform.Publishing.Domain.Model;

public static class PublishingErrors
{
    public static readonly Error CategoryCreationFailed = new("Publishing.CategoryCreationFailed", "An error occurred while creating the category.");
    public static readonly Error TutorialNotFound = new("Publishing.TutorialNotFound", "The specified tutorial was not found.");
    public static readonly Error CategoryNotFound = new("Publishing.CategoryNotFound", "The specified category was not found.");
    public static readonly Error DuplicateTutorialTitle = new("Publishing.DuplicateTutorialTitle", "A tutorial with the same title already exists.");
}
