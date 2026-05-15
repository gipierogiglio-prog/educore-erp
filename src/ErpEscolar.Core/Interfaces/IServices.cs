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
    Task<List<Services.TeacherResponse>> GetAllAsync(Guid orgId);
    Task<Services.TeacherResponse?> GetByIdAsync(Guid id);
    Task<Services.TeacherResponse> CreateAsync(Services.CreateTeacherRequest request, Guid orgId);
}

public interface IAcademicService
{
    Task<List<Services.ClassResponse>> GetClassesAsync(Guid orgId);
    Task<Services.ClassResponse> CreateClassAsync(string name, string shift, string? room, Guid orgId);
    Task<List<Services.SubjectResponse>> GetSubjectsAsync(Guid orgId);
    Task<Services.SubjectResponse> CreateSubjectAsync(string name, string code, int workload, Guid orgId);
    Task AssignTeacherAsync(Guid teacherId, Guid subjectId, Guid classId, Guid orgId);
    Task<List<Services.GradeResponse>> GetGradesByClassAsync(Guid classId, int bimester, int year);
    Task SubmitGradesAsync(Services.GradeBatchRequest request, Guid orgId);
    Task SubmitAttendanceAsync(Services.AttendanceBatchRequest request, Guid orgId);
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

public interface ICourseService
{
    Task<List<Services.CourseResponse>> GetAllAsync(Guid orgId);
    Task<Services.CourseResponse?> GetByIdAsync(Guid id);
    Task<Services.CourseResponse> CreateAsync(Services.CreateCourseRequest request, Guid orgId);
    Task<Services.CourseResponse> UpdateAsync(Guid id, Services.UpdateCourseRequest request);
    Task DeleteAsync(Guid id);
}

public interface IStaffService
{
    Task<List<Services.StaffResponse>> GetAllAsync(Guid orgId);
    Task<Services.StaffResponse?> GetByIdAsync(Guid id);
    Task<Services.StaffResponse> CreateAsync(Services.CreateStaffRequest request, Guid orgId);
    Task<Services.StaffResponse> UpdateAsync(Guid id, Services.UpdateStaffRequest request, Guid orgId);
    Task ToggleStatusAsync(Guid id);
}

public interface ILessonPlanService
{
    Task<List<Services.LessonPlanResponse>> GetByClassAsync(Guid classId, Guid? subjectId, int? month, int? year);
    Task<Services.LessonPlanResponse> CreateAsync(Services.CreateLessonPlanRequest request, Guid orgId, Guid teacherId);
    Task<Services.LessonPlanResponse> UpdateAsync(Guid id, Services.UpdateLessonPlanRequest request);
    Task DeleteAsync(Guid id);
}

public interface IAssessmentService
{
    Task<List<Services.AssessmentResponse>> GetByClassAsync(Guid classId, int? bimester, int? year);
    Task<Services.AssessmentResponse> CreateAsync(Services.CreateAssessmentRequest request, Guid orgId);
    Task SubmitGradesAsync(Guid assessmentId, List<Services.SubmitGradeItem> grades);
    Task<List<Services.AssessmentGradeResponse>> GetGradesAsync(Guid assessmentId);
}

public interface IScheduleService
{
    Task<List<Services.ScheduleResponse>> GetByClassAsync(Guid classId);
    Task<List<Services.ScheduleResponse>> GetByTeacherAsync(Guid teacherId);
    Task<Services.ScheduleResponse> CreateAsync(Services.CreateScheduleRequest request, Guid orgId);
    Task DeleteAsync(Guid id);
}

public interface IGradingRuleService
{
    Task<Services.GradingRuleResponse?> GetAsync(Guid orgId);
    Task<Services.GradingRuleResponse> SaveAsync(Services.UpsertGradingRuleRequest request, Guid orgId);
}
