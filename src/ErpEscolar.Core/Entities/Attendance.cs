namespace ErpEscolar.Core.Entities;

public class Attendance
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StudentId { get; set; }
    public Guid ClassId { get; set; }
    public Guid? SubjectId { get; set; }
    public DateTime Date { get; set; }
    public bool Present { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public string? Justification { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Student Student { get; set; } = null!;
    public Class Class { get; set; } = null!;
    public Subject? Subject { get; set; }
}
