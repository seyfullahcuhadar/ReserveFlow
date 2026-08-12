using ReserveFlow.Domain.Abstractions;

namespace ReserveFlow.Domain.Catalog;

public sealed class OrganizerProfile : AggregateRoot
{
    private OrganizerProfile(
        Guid id,
        Guid userId,
        string displayName,
        string? bio,
        DateTime createdAtUtc)
        : base(id, createdAtUtc)
    {
        UserId = userId;
        DisplayName = displayName;
        Bio = bio;
    }

    private OrganizerProfile()
    {
    }

    public Guid UserId { get; private set; }

    public string DisplayName { get; private set; } = null!;

    public string? Bio { get; private set; }

    public static OrganizerProfile Create(
        Guid userId,
        string displayName,
        string? bio,
        DateTime createdAtUtc)
    {

        var profile = new OrganizerProfile(
            Guid.NewGuid(),
            userId,
            displayName.Trim(),
            bio.Trim(),
            createdAtUtc);

        profile.RaiseDomainEvent(
            new OrganizerProfileCreatedDomainEvent(profile.Id, profile.UserId, createdAtUtc));

        return profile;
    }
}
