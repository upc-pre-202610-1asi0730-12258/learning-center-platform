using ACME.LearningCenterPlatform.API.Profiles.Domain.Commands;
using ACME.LearningCenterPlatform.API.Profiles.Domain.Model.ValueObjects;

namespace ACME.LearningCenterPlatform.API.Profiles.Domain.Aggregates;

/// <summary>
/// Profile aggregate root. 
/// </summary>
public partial class Profile
{
    public int Id { get; }
    public PersonName Name { get; private set; }
    public EmailAddress Email { get; private set; }
    public StreetAddress Address { get; private set; }

    public string FullName => Name.FullName;

    public string EmailAddress => Email.Address;

    public string StreetAddress => Address.FullAddress;
    
    /// <summary>
    /// Default constructor for EF Core. 
    /// </summary>
    public Profile()
    {
        Name = new PersonName();
        Email = new EmailAddress();
        Address = new StreetAddress();
    }

    /// <summary>
    /// Constructor for creating a new profile. 
    /// </summary>
    /// <param name="firstName">
    /// The first name of the person.
    /// </param>
    /// <param name="lastName">
    /// The last name of the person.
    /// </param>
    /// <param name="email">
    /// The email address of the person.
    /// </param>
    /// <param name="street">
    /// The street of the address.
    /// </param>
    /// <param name="number">
    /// The number of the address.
    /// </param>
    /// <param name="city">
    /// The city of the address.
    /// </param>
    /// <param name="postalCode">
    /// The postal code of the address.
    /// </param>
    /// <param name="country">
    /// The country of the address.
    /// </param>
    public Profile(string firstName, string lastName, string email, string street, string number, string city,
        string postalCode, string country)
    {
        Name = new PersonName(firstName, lastName);
        Email = new EmailAddress(email);
        Address = new StreetAddress(street, number, city, postalCode, country);
    }

    /// <summary>
    /// Constructor for creating a new profile. 
    /// </summary>
    /// <param name="command">
    /// The <see cref="CreateProfileCommand"/> command to create a new profile.
    /// </param>
    public Profile(CreateProfileCommand command)
    {
        Name = new PersonName(command.FirstName, command.LastName);
        Email = new EmailAddress(command.Email);
        Address = new StreetAddress(command.Street, command.Number, command.City, command.PostalCode, command.Country);
    }
}