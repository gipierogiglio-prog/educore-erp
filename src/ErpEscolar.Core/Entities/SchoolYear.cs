namespace ErpEscolar.Core.Entities;

public class SchoolYear
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Year { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "planned"; // planned, active, completed, cancelled
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
}
