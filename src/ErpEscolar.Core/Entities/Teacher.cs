namespace ErpEscolar.Core.Entities;

public class Teacher
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string? Specialization { get; set; }
    public DateTime HireDate { get; set; } = DateTime.UtcNow;
    public bool Active { get; set; } = true;

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
}
