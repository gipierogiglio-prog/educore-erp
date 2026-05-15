namespace ErpEscolar.Core.Entities;

public class GradingRule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public string Name { get; set; } = "Regra Padrao";  // Nome da regra
    public decimal PassingGrade { get; set; } = 6m;      // Nota para aprovacao
    public decimal RecoveryGrade { get; set; } = 3m;     // Nota minima para recuperacao
    public decimal B1Weight { get; set; } = 1m;          // Peso do 1o bimestre
    public decimal B2Weight { get; set; } = 1m;          // Peso do 2o bimestre
    public decimal B3Weight { get; set; } = 1m;          // Peso do 3o bimestre
    public decimal B4Weight { get; set; } = 1m;          // Peso do 4o bimestre
    public bool UseRecoveryExam { get; set; } = true;    // Prova de recuperacao?
    public decimal RecoveryMaxScore { get; set; } = 10m; // Nota maxima da recuperacao
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
