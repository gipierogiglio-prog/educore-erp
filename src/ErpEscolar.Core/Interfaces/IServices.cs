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
    Task<Services.ClassResponse> UpdateClassAsync(Guid id, string name, string shift, string? room, Guid? courseId, Guid orgId);
    Task DeleteClassAsync(Guid id);
    Task<List<Services.SubjectResponse>> GetSubjectsAsync(Guid orgId);
    Task<Services.SubjectResponse> CreateSubjectAsync(string name, string code, int workload, Guid orgId);
    Task<Services.SubjectResponse> UpdateSubjectAsync(Guid id, string name, string code, int? workload, Guid? courseId, Guid orgId);
    Task DeleteSubjectAsync(Guid id);
    Task AssignTeacherAsync(Guid teacherId, Guid subjectId, Guid classId, Guid orgId);
    Task<List<Services.GradeResponse>> GetGradesByClassAsync(Guid classId, int bimester, int year);
    Task SubmitGradesAsync(Services.GradeBatchRequest request, Guid orgId);
    Task SubmitAttendanceAsync(Services.AttendanceBatchRequest request, Guid orgId);
    Task<Services.StudentReportCard> GetStudentReportCardAsync(Guid studentId, int year);
    Task<List<Services.StudentAttendanceSummary>> GetAttendanceSummaryAsync(Guid classId, Guid subjectId, int year, int? month);
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
    Task<TransferResponse> TransferAsync(TransferRequest request, Guid orgId);
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
    Task<List<CourseResponse>> GetAllAsync(Guid orgId);
    Task<CourseResponse?> GetByIdAsync(Guid id);
    Task<CourseResponse> CreateAsync(CreateCourseRequest request, Guid orgId);
    Task<CourseResponse> UpdateAsync(Guid id, UpdateCourseRequest request);
    Task DeleteAsync(Guid id);
}

public interface IStaffService
{
    Task<List<StaffResponse>> GetAllAsync(Guid orgId);
    Task<StaffResponse?> GetByIdAsync(Guid id);
    Task<StaffResponse> CreateAsync(CreateStaffRequest request, Guid orgId);
    Task<StaffResponse> UpdateAsync(Guid id, UpdateStaffRequest request, Guid orgId);
    Task ToggleStatusAsync(Guid id);
}

public interface ILessonPlanService
{
    Task<List<LessonPlanResponse>> GetByClassAsync(Guid classId, Guid? subjectId, int? month, int? year);
    Task<LessonPlanResponse> CreateAsync(CreateLessonPlanRequest request, Guid orgId, Guid teacherId);
    Task<LessonPlanResponse> UpdateAsync(Guid id, UpdateLessonPlanRequest request);
    Task DeleteAsync(Guid id);
}

public interface IAssessmentService
{
    Task<List<AssessmentResponse>> GetByClassAsync(Guid classId, int? bimester, int? year);
    Task<AssessmentResponse> CreateAsync(CreateAssessmentRequest request, Guid orgId);
    Task SubmitGradesAsync(Guid assessmentId, List<SubmitGradeItem> grades);
    Task<List<AssessmentGradeResponse>> GetGradesAsync(Guid assessmentId);
}

public interface IScheduleService
{
    Task<List<ScheduleResponse>> GetByClassAsync(Guid classId);
    Task<List<ScheduleResponse>> GetByTeacherAsync(Guid teacherId);
    Task<ScheduleResponse> CreateAsync(CreateScheduleRequest request, Guid orgId);
    Task DeleteAsync(Guid id);
}

public interface IGradingRuleService
{
    Task<GradingRuleResponse?> GetAsync(Guid orgId);
    Task<GradingRuleResponse> SaveAsync(UpsertGradingRuleRequest request, Guid orgId);
}
