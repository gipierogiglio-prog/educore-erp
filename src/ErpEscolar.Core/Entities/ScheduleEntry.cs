namespace ErpEscolar.Core.Entities;

public class ScheduleEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClassId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid TeacherId { get; set; }
    public int DayOfWeek { get; set; }                 // 0=Sunday, 1=Monday... 6=Saturday
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Room { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    // Navigation
    public Class Class { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
    public Teacher Teacher { get; set; } = null!;
}
