namespace ErpEscolar.Core.Entities;

public class Enrollment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StudentId { get; set; }
    public Guid ClassId { get; set; }
    public int SchoolYear { get; set; } = DateTime.UtcNow.Year;
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "active"; // active, transferred, completed, cancelled
    public DateTime? EndDate { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public Student Student { get; set; } = null!;
    public Class Class { get; set; } = null!;
}
