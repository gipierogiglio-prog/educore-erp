namespace ErpEscolar.Core.Entities;

public class TuitionPlan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public int DueDay { get; set; } = 10;
    public string? Description { get; set; }
    public bool Active { get; set; } = true;

    // Navigation
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
