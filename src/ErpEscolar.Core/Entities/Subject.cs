namespace ErpEscolar.Core.Entities;

public class Subject
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int Workload { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public Guid? CourseId { get; set; }
    public Course? Course { get; set; }
    public string? Description { get; set; }
    public bool Active { get; set; } = true;

    // Navigation
    public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    public ICollection<Class> Classes { get; set; } = new List<Class>();
}
