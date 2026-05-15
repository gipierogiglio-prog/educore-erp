using ErpEscolar.Core.Entities;
using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace ErpEscolar.Infra.Services;

public class AcademicService : IAcademicService
{
    private readonly IClassRepository _classRepo;
    private readonly ISubjectRepository _subjectRepo;
    private readonly IGradeRepository _gradeRepo;
    private readonly IAttendanceRepository _attendanceRepo;
    private readonly IStudentRepository _studentRepo;
    private readonly ITeacherRepository _teacherRepo;
    private readonly IEnrollmentRepository _enrollmentRepo;

    public AcademicService(
        IClassRepository classRepo, ISubjectRepository subjectRepo,
        IGradeRepository gradeRepo, IAttendanceRepository attendanceRepo,
        IStudentRepository studentRepo, ITeacherRepository teacherRepo,
        IEnrollmentRepository enrollmentRepo)
    {
        _classRepo = classRepo;
        _subjectRepo = subjectRepo;
        _gradeRepo = gradeRepo;
        _attendanceRepo = attendanceRepo;
        _studentRepo = studentRepo;
        _teacherRepo = teacherRepo;
        _enrollmentRepo = enrollmentRepo;
    }

    // === Classes ===

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

    public async Task<ClassResponse> UpdateClassAsync(Guid id, string name, string shift, string? room, Guid? courseId, Guid orgId)
    {
        var c = await _classRepo.GetByIdAsync(id);
        if (c == null) throw new KeyNotFoundException("Turma não encontrada");
        if (c.OrganizationId != orgId) throw new UnauthorizedAccessException();

        c.Name = name;
        c.Shift = shift;
        c.Room = room;
        c.CourseId = courseId;
        await _classRepo.UpdateAsync(c);
        return new ClassResponse(c.Id, c.Name, c.Shift, c.Year, c.Students.Count(s => s.Active));
    }

    public async Task DeleteClassAsync(Guid id)
    {
        var c = await _classRepo.GetByIdAsync(id);
        if (c == null) throw new KeyNotFoundException("Turma não encontrada");
        c.Active = false;
        await _classRepo.UpdateAsync(c);
    }

    // === Subjects ===

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

    public async Task<SubjectResponse> UpdateSubjectAsync(Guid id, string name, string code, int? workload, Guid? courseId, Guid orgId)
    {
        var s = await _subjectRepo.GetByIdAsync(id);
        if (s == null) throw new KeyNotFoundException("Disciplina não encontrada");
        if (s.OrganizationId != orgId) throw new UnauthorizedAccessException();

        s.Name = name;
        s.Code = code;
        if (workload.HasValue) s.Workload = workload.Value;
        s.CourseId = courseId;
        await _subjectRepo.UpdateAsync(s);
        return new SubjectResponse(s.Id, s.Name, s.Code, s.Workload);
    }

    public async Task DeleteSubjectAsync(Guid id)
    {
        var s = await _subjectRepo.GetByIdAsync(id);
        if (s == null) throw new KeyNotFoundException("Disciplina não encontrada");
        await _subjectRepo.DeleteAsync(id);
    }

    // === Teacher Assignment ===

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

    // === Grades ===

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
                OrganizationId = orgId
            });
        }

        if (grades.Count > 0)
            await _gradeRepo.CreateBatchAsync(grades);
    }

    // === Attendance ===

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
            OrganizationId = orgId
        }).ToList();

        await _attendanceRepo.CreateBatchAsync(attendances);
    }

    // === Report Card ===

    public async Task<StudentReportCard> GetStudentReportCardAsync(Guid studentId, int year)
    {
        var student = await _studentRepo.GetByIdAsync(studentId);
        if (student == null) throw new KeyNotFoundException("Aluno não encontrado");

        var enrollment = await _enrollmentRepo.GetByStudentAsync(studentId);
        var currentEnrollment = enrollment.FirstOrDefault(e => e.SchoolYear == year);

        var grades = await _gradeRepo.GetByStudentAndYearAsync(studentId, year);
        var subjectsByBimester = grades.GroupBy(g => new { g.SubjectId, SubjectName = g.Subject.Name });

        var subjectGrades = new List<SubjectGrade>();
        foreach (var subject in subjectsByBimester.OrderBy(s => s.Key.SubjectName))
        {
            var b1 = GetBimesterGrade(subject, 1);
            var b2 = GetBimesterGrade(subject, 2);
            var b3 = GetBimesterGrade(subject, 3);
            var b4 = GetBimesterGrade(subject, 4);

            var avg = (GetFinalValue(b1) + GetFinalValue(b2) + GetFinalValue(b3) + GetFinalValue(b4)) / 4m;
            var status = avg >= 6 ? "Aprovado" : avg >= 3 ? "Recuperação" : "Reprovado";

            subjectGrades.Add(new SubjectGrade(
                subject.Key.SubjectName,
                b1.Value, b1.Recovery, b2.Value, b2.Recovery,
                b3.Value, b3.Recovery, b4.Value, b4.Recovery,
                Math.Round(avg, 1), status
            ));
        }

        var overallAvg = subjectGrades.Any()
            ? Math.Round(subjectGrades.Average(s => s.Average), 1)
            : 0;

        return new StudentReportCard(
            student.User.Name,
            currentEnrollment?.ClassName ?? student.Class?.Name ?? "-",
            year, subjectGrades, overallAvg
        );
    }

    public async Task<List<StudentAttendanceSummary>> GetAttendanceSummaryAsync(Guid classId, Guid subjectId, int year, int? month)
    {
        var students = await _studentRepo.GetByClassIdAsync(classId);
        var result = new List<StudentAttendanceSummary>();

        foreach (var student in students)
        {
            var records = await _attendanceRepo.GetByStudentAndSubjectAsync(student.Id, subjectId, year);
            if (month.HasValue)
                records = records.Where(a => a.Date.Month == month.Value).ToList();

            var total = records.Count;
            var present = records.Count(a => a.Present);

            result.Add(new StudentAttendanceSummary(
                student.Id, student.User.Name,
                total, present, total - present,
                total > 0 ? Math.Round((double)present / total * 100, 1) : 0
            ));
        }

        return result.OrderBy(s => s.StudentName).ToList();
    }

    // === Helpers ===
    private static (decimal Value, decimal? Recovery) GetBimesterGrade(IGrouping<object, Grade> subject, int bimester)
    {
        var g = subject.FirstOrDefault(x => x.Bimester == bimester);
        return g != null ? (g.Value, g.RecoveryValue) : (0, null);
    }

    private static decimal GetFinalValue((decimal Value, decimal? Recovery) b)
    {
        return b.Recovery.HasValue && b.Recovery > b.Value ? b.Recovery.Value : b.Value;
    }
}
