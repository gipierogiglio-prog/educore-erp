using ErpEscolar.Core.Entities;
using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;

namespace ErpEscolar.Infra.Services;

public class AcademicService : IAcademicService
{
    private readonly IClassRepository _classRepo;
    private readonly ISubjectRepository _subjectRepo;
    private readonly IGradeRepository _gradeRepo;
    private readonly IAttendanceRepository _attendanceRepo;
    private readonly IStudentRepository _studentRepo;
    private readonly ITeacherRepository _teacherRepo;

    public AcademicService(
        IClassRepository classRepo, ISubjectRepository subjectRepo,
        IGradeRepository gradeRepo, IAttendanceRepository attendanceRepo,
        IStudentRepository studentRepo, ITeacherRepository teacherRepo)
    {
        _classRepo = classRepo;
        _subjectRepo = subjectRepo;
        _gradeRepo = gradeRepo;
        _attendanceRepo = attendanceRepo;
        _studentRepo = studentRepo;
        _teacherRepo = teacherRepo;
    }

    public async Task<List<ClassResponse>> GetClassesAsync(Guid orgId)
    {
        var classes = await _classRepo.GetAllAsync(orgId: orgId);
        return classes.Select(c => new ClassResponse(c.Id, c.Name, c.Shift, c.Year,
            c.Students.Count(s => s.Active))).ToList();
    }

    public async Task<ClassResponse> CreateClassAsync(string name, string shift, string? room, Guid orgId)
    {
        var c = new Class { Name = name, Shift = shift, Room = room, OrganizationId = orgId };
        c = await _classRepo.CreateAsync(c);
        return new ClassResponse(c.Id, c.Name, c.Shift, c.Year, 0);
    }

    public async Task<List<SubjectResponse>> GetSubjectsAsync(Guid orgId)
    {
        var subjects = await _subjectRepo.GetAllAsync(orgId: orgId);
        return subjects.Select(s => new SubjectResponse(s.Id, s.Name, s.Code, s.Workload)).ToList();
    }

    public async Task<SubjectResponse> CreateSubjectAsync(string name, string code, int workload, Guid orgId)
    {
        var s = new Subject { Name = name, Code = code, Workload = workload, OrganizationId = orgId };
        s = await _subjectRepo.CreateAsync(s);
        return new SubjectResponse(s.Id, s.Name, s.Code, s.Workload);
    }

    public async Task AssignTeacherAsync(Guid teacherId, Guid subjectId, Guid classId, Guid orgId)
    {
        var teacher = await _teacherRepo.GetByIdAsync(teacherId);
        if (teacher == null) throw new KeyNotFoundException("Professor não encontrado");

        if (teacher.TeacherSubjects.Any(ts => ts.SubjectId == subjectId && ts.ClassId == classId))
            throw new InvalidOperationException("Professor já atribuído a esta disciplina/turma");

        teacher.TeacherSubjects.Add(new TeacherSubject
        {
            TeacherId = teacherId,
            SubjectId = subjectId,
            ClassId = classId,
        });
        await _teacherRepo.UpdateAsync(teacher);
    }

    public async Task<List<GradeResponse>> GetGradesByClassAsync(Guid classId, int bimester, int year)
    {
        var grades = await _gradeRepo.GetByClassAndBimesterAsync(classId, bimester, year);
        return grades.Select(g => new GradeResponse(
            g.StudentId, g.Student.User.Name, g.Bimester, g.Value, g.RecoveryValue)).ToList();
    }

    public async Task SubmitGradesAsync(GradeBatchRequest request, Guid orgId)
    {
        var students = await _studentRepo.GetByClassIdAsync(request.ClassId);
        var grades = new List<Grade>();

        foreach (var item in request.Grades)
        {
            if (!students.Any(s => s.Id == item.StudentId)) continue;

            grades.Add(new Grade
            {
                StudentId = item.StudentId,
                SubjectId = request.SubjectId,
                Bimester = request.Bimester,
                Value = item.Value,
                RecoveryValue = item.RecoveryValue,
                SchoolYear = request.Year,
            });
        }

        if (grades.Count > 0)
            await _gradeRepo.CreateBatchAsync(grades);
    }

    public async Task SubmitAttendanceAsync(AttendanceBatchRequest request, Guid orgId)
    {
        var attendances = request.Attendances.Select(a => new Attendance
        {
            StudentId = a.StudentId,
            ClassId = request.ClassId,
            SubjectId = request.SubjectId,
            Date = request.Date,
            Present = a.Present,
            Justification = a.Justification,
        }).ToList();

        await _attendanceRepo.CreateBatchAsync(attendances);
    }
}
