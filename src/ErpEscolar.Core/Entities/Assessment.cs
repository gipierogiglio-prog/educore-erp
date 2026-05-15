namespace ErpEscolar.Core.Entities;

public class Assessment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;     // "Prova Bimestral", "Trabalho"
    public string Type { get; set; } = "exam";             // exam, test, homework, project, recovery
    public Guid SubjectId { get; set; }
    public Guid ClassId { get; set; }
    public int Bimester { get; set; }
    public int SchoolYear { get; set; } = DateTime.UtcNow.Year;
    public DateTime Date { get; set; }
    public decimal MaxScore { get; set; } = 10m;           // Nota maxima
    public decimal Weight { get; set; } = 1m;              // Peso na media do bimestre
    public string? Description { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Subject Subject { get; set; } = null!;
    public Class Class { get; set; } = null!;
}
