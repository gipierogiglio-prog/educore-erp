namespace ErpEscolar.Core.Services;

public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token, string Name, string Role, DateTime ExpiresAt);

public record RegisterRequest(string Name, string Email, string Password, string Role);

public record CreateStudentRequest(
    string Name, string Email, string Password,
    string? Phone, Guid? ClassId, string? GuardianName, string? GuardianPhone, string? GuardianRelationship
);

public record CreateTeacherRequest(
    string Name, string Email, string Password,
    string? Phone, string? Specialization
);

public record GradeBatchItem(Guid StudentId, decimal Value, decimal? RecoveryValue);
public record GradeBatchRequest(Guid SubjectId, Guid ClassId, int Bimester, int Year, List<GradeBatchItem> Grades);

public record AttendanceBatchItem(Guid StudentId, bool Present, string? Justification);
public record AttendanceBatchRequest(Guid ClassId, Guid? SubjectId, DateTime Date, List<AttendanceBatchItem> Attendances);

public record StudentResponse(
    Guid Id, string Name, string Email, string Enrollment,
    Guid? ClassId, string? ClassName, string Status, string? GuardianName
);

public record TeacherResponse(
    Guid Id, string Name, string Email, string? Specialization, int SubjectCount
);

public record ClassResponse(Guid Id, string Name, string Shift, int Year, int StudentCount);

public record SubjectResponse(Guid Id, string Name, string Code, int Workload);

public record GradeResponse(Guid StudentId, string StudentName, int Bimester, decimal Value, decimal? Recovery);

public record DashboardData(
    int TotalStudents, int TotalTeachers, int TotalClasses,
    int PendingInvoices, decimal MonthlyRevenue, decimal OverdueAmount
);
