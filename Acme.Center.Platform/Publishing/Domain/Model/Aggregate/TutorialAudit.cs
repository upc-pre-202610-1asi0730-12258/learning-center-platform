using Acme.Center.Platform.Shared.Domain.Model.Entities;

namespace Acme.Center.Platform.Publishing.Domain.Model.Aggregate;

public partial class Tutorial : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}