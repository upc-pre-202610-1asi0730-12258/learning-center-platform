namespace ACME.LearningCenterPlatform.API.Profiles.Domain.Model.ValueObjects;

/// <summary>
///     Value Object that represents an email address.
/// </summary>
/// <param name="Address">
///     The email address.
/// </param>
public record EmailAddress(string Address)
{
    public EmailAddress() : this(string.Empty)
    {
    }
}