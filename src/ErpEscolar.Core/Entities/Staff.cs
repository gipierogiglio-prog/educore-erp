namespace ErpEscolar.Core.Entities;

public class Staff
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Position { get; set; } = string.Empty;  // secretary, coordinator, principal, financial, etc
    public string? Department { get; set; }                // administrative, pedagogical, financial
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public DateTime HireDate { get; set; } = DateTime.UtcNow;
    public decimal? Salary { get; set; }
    public bool Active { get; set; } = true;

    // Navigation
    public User User { get; set; } = null!;
}
