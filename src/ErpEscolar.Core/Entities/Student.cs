namespace ErpEscolar.Core.Entities;

public class Student
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Enrollment { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public Guid? ClassId { get; set; }
    public Guid? GuardianId { get; set; }
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    public bool Active { get; set; } = true;

    // Navigation
    public User User { get; set; } = null!;
    public Class? Class { get; set; }
    public Guardian? Guardian { get; set; }
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
