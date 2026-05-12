namespace ErpEscolar.Core.Entities;

public class Grade
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StudentId { get; set; }
    public Guid SubjectId { get; set; }
    public int Bimester { get; set; }
    public decimal Value { get; set; }
    public decimal? RecoveryValue { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public int SchoolYear { get; set; } = DateTime.UtcNow.Year;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Student Student { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
}
