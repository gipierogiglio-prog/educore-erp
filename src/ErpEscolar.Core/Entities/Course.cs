namespace ErpEscolar.Core.Entities;

public class Course
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;        // e.g., "Ensino Médio", "Ensino Fundamental I"
    public string? Description { get; set; }
    public int DurationYears { get; set; } = 1;             // Quantidade de anos do curso
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ICollection<Class> Classes { get; set; } = new List<Class>();
    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
