namespace ErpEscolar.Core.Entities;

public class LessonPlan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClassId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid TeacherId { get; set; }
    public DateTime Date { get; set; }
    public string? Topic { get; set; }
    public string? Objectives { get; set; }         // Objetivos da aula
    public string? Content { get; set; }             // Conteudo programatico
    public string? Methodology { get; set; }         // Metodologia
    public string? Resources { get; set; }           // Recursos necessarios
    public string? Homework { get; set; }            // Tarefa de casa
    public string? Notes { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Class Class { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
    public Teacher Teacher { get; set; } = null!;
}
