namespace ErpEscolar.Core.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "teacher";
    // super_admin = dono do sistema
    // org_admin = admin da escola
    // teacher, student, guardian
    public bool Active { get; set; } = true;
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public Guid? OrganizationId { get; set; } // null = super_admin
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Organization? Organization { get; set; }
    public Teacher? Teacher { get; set; }
    public Student? Student { get; set; }
    public ICollection<Guardian>? Guardians { get; set; }
}
