namespace ErpEscolar.Core.Entities;

public class TeacherSubject
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TeacherId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid ClassId { get; set; }

    // Navigation
    public Teacher Teacher { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
    public Class Class { get; set; } = null!;
}
