namespace ErpEscolar.Core.Entities;

public class AssessmentGrade
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AssessmentId { get; set; }
    public Guid StudentId { get; set; }
    public decimal Score { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public Assessment Assessment { get; set; } = null!;
    public Student Student { get; set; } = null!;
}
