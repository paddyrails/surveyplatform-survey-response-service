namespace SurveyPlatform.SurveyResponseService.Domain.Aggregates;

public abstract class Entity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public string CreatedBy { get; protected set; } = string.Empty;
    public string? UpdatedBy { get; protected set; }

    protected Entity() { Id = Guid.NewGuid(); CreatedAt = DateTime.UtcNow; }
    protected Entity(Guid id) { Id = id; CreatedAt = DateTime.UtcNow; }

    public override bool Equals(object? obj) => obj is Entity e && Id == e.Id;
    public override int GetHashCode() => Id.GetHashCode();
}
