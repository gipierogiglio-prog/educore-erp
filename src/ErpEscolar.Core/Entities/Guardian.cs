namespace ErpEscolar.Core.Entities;

public class Guardian
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Relationship { get; set; } = string.Empty; // father, mother, uncle, etc
    public string? Phone { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<Student> Students { get; set; } = new List<Student>();
}
