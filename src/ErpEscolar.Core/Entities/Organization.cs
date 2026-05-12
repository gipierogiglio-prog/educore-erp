namespace ErpEscolar.Core.Entities;

public class Organization
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Document { get; set; } // CNPJ/CPF
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Status { get; set; } = "active"; // active, inactive, suspended
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
