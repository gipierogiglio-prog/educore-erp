namespace ErpEscolar.Core.Interfaces;

public interface IAuthService
{
    Task<Services.LoginResponse> LoginAsync(Services.LoginRequest request);
    Task<Services.LoginResponse> RegisterAsync(Services.RegisterRequest request);
}

public interface IStudentService
{
    Task<List<Services.StudentResponse>> GetAllAsync(Guid orgId);
    Task<Services.StudentResponse?> GetByIdAsync(Guid id);
    Task<Services.StudentResponse> CreateAsync(Services.CreateStudentRequest request, Guid orgId);
    Task<bool> ToggleStatusAsync(Guid id);
    Task UpdateClassAsync(Guid studentId, Guid? classId);
}

public interface ITeacherService
{
    Task<List<Services.TeacherResponse>> GetAllAsync();
    Task<Services.TeacherResponse?> GetByIdAsync(Guid id);
    Task<Services.TeacherResponse> CreateAsync(Services.CreateTeacherRequest request);
}

public interface IAcademicService
{
    Task<List<Services.ClassResponse>> GetClassesAsync();
    Task<Services.ClassResponse> CreateClassAsync(string name, string shift, string? room);
    Task<List<Services.SubjectResponse>> GetSubjectsAsync();
    Task<Services.SubjectResponse> CreateSubjectAsync(string name, string code, int workload);
    Task AssignTeacherAsync(Guid teacherId, Guid subjectId, Guid classId);
    Task<List<Services.GradeResponse>> GetGradesByClassAsync(Guid classId, int bimester, int year);
    Task SubmitGradesAsync(Services.GradeBatchRequest request);
    Task SubmitAttendanceAsync(Services.AttendanceBatchRequest request);
}

public interface IFinancialService
{
    Task<List<Services.StudentResponse>> GetDebtorsAsync();
    Task RegisterPaymentAsync(Guid invoiceId, string method);
    Task GenerateMonthlyInvoicesAsync(int year, int month);
}

public interface ISchoolYearService
{
    Task<List<Services.SchoolYearResponse>> GetAllAsync();
    Task<Services.SchoolYearResponse?> GetByIdAsync(Guid id);
    Task<Services.SchoolYearResponse> CreateAsync(Services.CreateSchoolYearRequest request);
    Task UpdateStatusAsync(Guid id, string status);
}

public interface IEnrollmentService
{
    Task<List<Services.EnrollmentResponse>> GetAllAsync(int? year = null);
    Task<List<Services.EnrollmentResponse>> GetByStudentAsync(Guid studentId);
    Task<Services.EnrollmentResponse> CreateAsync(Services.CreateEnrollmentRequest request);
    Task UpdateStatusAsync(Guid id, string status);
}

public interface IDashboardService
{
    Task<Services.DashboardData> GetDashboardAsync();
}
