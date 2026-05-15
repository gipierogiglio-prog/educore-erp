namespace ErpEscolar.Core.Services;

public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token, string Name, string Role, DateTime ExpiresAt);

public record RegisterRequest(string Name, string Email, string Password, string Role, Guid? OrganizationId = null);

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

public record EnrollmentResponse(
    Guid Id, Guid StudentId, string StudentName, string StudentEnrollment,
    Guid ClassId, string ClassName, int SchoolYear, string Status, DateTime EnrollmentDate
);

public record SchoolYearResponse(
    Guid Id, int Year, string? Description, DateTime StartDate, DateTime EndDate, string Status
);

public record CreateSchoolYearRequest(
    int Year, DateTime StartDate, DateTime EndDate, string? Description
);

public record CreateEnrollmentRequest(
    Guid StudentId, Guid ClassId, int? SchoolYear, string? Notes
);

public record DashboardData(
    int TotalStudents, int TotalTeachers, int TotalClasses,
    int PendingInvoices, decimal MonthlyRevenue, decimal OverdueAmount
);

public record CreateCourseRequest(string Name, string? Description, int DurationYears);
public record UpdateCourseRequest(string? Name, string? Description, int? DurationYears);
public record CourseResponse(
    Guid Id, string Name, string? Description, int DurationYears,
    int ClassCount, int SubjectCount, bool Active, DateTime CreatedAt
);

// === Report Card ===
public record SubjectGrade(
    string SubjectName, decimal B1, decimal? R1, decimal B2, decimal? R2,
    decimal B3, decimal? R3, decimal B4, decimal? R4, decimal Average, string Status
);

public record StudentReportCard(
    string StudentName, string ClassName, int Year,
    List<SubjectGrade> Subjects, decimal OverallAverage
);

// === Attendance Summary ===
public record StudentAttendanceSummary(
    Guid StudentId, string StudentName,
    int TotalClasses, int PresentCount, int AbsentCount, double Percentage
);

// === Staff ===
public record CreateStaffRequest(
    string Name, string Email, string Password,
    string Position, string? Department, string? Phone, decimal? Salary
);
public record UpdateStaffRequest(
    string? Name, string? Position, string? Department, string? Phone, decimal? Salary
);
public record StaffResponse(
    Guid Id, string Name, string Email, string Position,
    string? Department, string? Phone, DateTime HireDate, bool Active
);

// === Transfer ===
public record TransferRequest(Guid StudentId, Guid FromClassId, Guid ToClassId, string? Reason);
public record TransferResponse(
    Guid EnrollmentId, string StudentName, string FromClass, string ToClass,
    DateTime Date, string? Reason, string Status
);

// === Reports ===
public record EnrolledStudentsReport(
    string StudentName, string Enrollment, string ClassName,
    string Shift, string? GuardianName, string Status
);
public record ClassesByYearReport(string ClassName, string Shift, int StudentCount, int Vacancies);
public record StudentsByCourseReport(string CourseName, int ClassCount, int StudentCount);
