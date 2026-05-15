using ErpEscolar.Core.Entities;
using ErpEscolar.Core.Interfaces;
using ErpEscolar.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace ErpEscolar.Infra.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public async Task<User?> GetByIdAsync(Guid id) =>
        await _db.Users.FindAsync(id);

    public async Task<User?> GetByEmailAsync(string email) =>
        await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User> CreateAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _db;
    public StudentRepository(AppDbContext db) => _db = db;

    public async Task<Student?> GetByIdAsync(Guid id) =>
        await _db.Students.Include(s => s.User).Include(s => s.Class).Include(s => s.Guardian).ThenInclude(g => g!.User)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<List<Student>> GetAllAsync(Guid orgId, bool activeOnly = true)
    {
        var query = _db.Students.Include(s => s.User).Include(s => s.Class)
            .Where(s => s.OrganizationId == orgId).AsQueryable();
        if (activeOnly) query = query.Where(s => s.Active);
        return await query.OrderBy(s => s.User.Name).ToListAsync();
    }

    public async Task<List<Student>> GetByClassIdAsync(Guid classId) =>
        await _db.Students.Include(s => s.User).Where(s => s.ClassId == classId && s.Active)
            .OrderBy(s => s.User.Name).ToListAsync();

    public async Task<Student> CreateAsync(Student student)
    {
        _db.Students.Add(student);
        await _db.SaveChangesAsync();
        return student;
    }

    public async Task UpdateAsync(Student student) => await _db.SaveChangesAsync();
}

public class TeacherRepository : ITeacherRepository
{
    private readonly AppDbContext _db;
    public TeacherRepository(AppDbContext db) => _db = db;

    public async Task<Teacher?> GetByIdAsync(Guid id) =>
        await _db.Teachers.Include(t => t.User).Include(t => t.TeacherSubjects).FirstOrDefaultAsync(t => t.Id == id);

    public async Task<List<Teacher>> GetAllAsync(bool activeOnly = true, Guid? orgId = null)
    {
        var query = _db.Teachers.Include(t => t.User).Include(t => t.TeacherSubjects).AsQueryable();
        if (activeOnly) query = query.Where(t => t.Active);
        if (orgId.HasValue) query = query.Where(t => t.OrganizationId == orgId.Value);
        return await query.OrderBy(t => t.User.Name).ToListAsync();
    }

    public async Task<Teacher> CreateAsync(Teacher teacher)
    {
        _db.Teachers.Add(teacher);
        await _db.SaveChangesAsync();
        return teacher;
    }

    public async Task UpdateAsync(Teacher teacher) => await _db.SaveChangesAsync();
}

public class ClassRepository : IClassRepository
{
    private readonly AppDbContext _db;
    public ClassRepository(AppDbContext db) => _db = db;

    public async Task<Class?> GetByIdAsync(Guid id) =>
        await _db.Classes.Include(c => c.Students).Include(c => c.TeacherSubjects)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<List<Class>> GetAllAsync(int? year = null, Guid? orgId = null)
    {
        var query = _db.Classes.Include(c => c.Students).AsQueryable();
        if (year.HasValue) query = query.Where(c => c.Year == year.Value);
        if (orgId.HasValue) query = query.Where(c => c.OrganizationId == orgId.Value);
        return await query.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<Class> CreateAsync(Class classEntity)
    {
        _db.Classes.Add(classEntity);
        await _db.SaveChangesAsync();
        return classEntity;
    }

    public async Task UpdateAsync(Class classEntity) => await _db.SaveChangesAsync();
}

public class SubjectRepository : ISubjectRepository
{
    private readonly AppDbContext _db;
    public SubjectRepository(AppDbContext db) => _db = db;

    public async Task<Subject?> GetByIdAsync(Guid id) => await _db.Subjects.FindAsync(id);
    public async Task<List<Subject>> GetAllAsync(Guid? orgId = null)
    {
        var query = _db.Subjects.AsQueryable();
        if (orgId.HasValue) query = query.Where(s => s.OrganizationId == orgId.Value);
        return await query.OrderBy(s => s.Name).ToListAsync();
    }

    public async Task<Subject> CreateAsync(Subject subject)
    {
        _db.Subjects.Add(subject);
        await _db.SaveChangesAsync();
        return subject;
    }

    public async Task UpdateAsync(Subject subject)
    {
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var subject = await _db.Subjects.FindAsync(id);
        if (subject != null)
        {
            subject.Active = false;
            await _db.SaveChangesAsync();
        }
    }


public class GradeRepository : IGradeRepository
{
    private readonly AppDbContext _db;
    public GradeRepository(AppDbContext db) => _db = db;

    public async Task<Grade?> GetByIdAsync(Guid id) => await _db.Grades.FindAsync(id);

    public async Task<List<Grade>> GetByStudentAsync(Guid studentId) =>
        await _db.Grades.Include(g => g.Subject).Where(g => g.StudentId == studentId)
            .OrderBy(g => g.Bimester).ToListAsync();

    public async Task<List<Grade>> GetByClassAndBimesterAsync(Guid classId, int bimester, int year) =>
        await _db.Grades.Include(g => g.Student).ThenInclude(s => s.User)
            .Where(g => g.Student.ClassId == classId && g.Bimester == bimester && g.SchoolYear == year)
            .ToListAsync();

    public async Task<Grade> CreateAsync(Grade grade)
    {
        _db.Grades.Add(grade);
        await _db.SaveChangesAsync();
        return grade;
    }

    public async Task CreateBatchAsync(List<Grade> grades)
    {
        _db.Grades.AddRange(grades);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Grade grade) => await _db.SaveChangesAsync();
}

public class AttendanceRepository : IAttendanceRepository
{
    private readonly AppDbContext _db;
    public AttendanceRepository(AppDbContext db) => _db = db;

    public async Task<Attendance?> GetByIdAsync(Guid id) => await _db.Attendances.FindAsync(id);

    public async Task<List<Attendance>> GetByClassAndDateAsync(Guid classId, DateTime date) =>
        await _db.Attendances.Include(a => a.Student).ThenInclude(s => s.User)
            .Where(a => a.ClassId == classId && a.Date.Date == date.Date)
            .OrderBy(a => a.Student.User.Name).ToListAsync();

    public async Task CreateBatchAsync(List<Attendance> attendances)
    {
        _db.Attendances.AddRange(attendances);
        await _db.SaveChangesAsync();
    }

    public async Task<List<Attendance>> GetByStudentAndSubjectAsync(Guid studentId, Guid subjectId, int year)
    {
        return await _db.Attendances
            .Where(a => a.StudentId == studentId && a.SubjectId == subjectId && a.Date.Year == year)
            .OrderBy(a => a.Date)
            .ToListAsync();
    }


public class SchoolYearRepository : ISchoolYearRepository
{
    private readonly AppDbContext _db;
    public SchoolYearRepository(AppDbContext db) => _db = db;

    public async Task<List<SchoolYear>> GetAllAsync() =>
        await _db.SchoolYears.OrderByDescending(sy => sy.Year).ToListAsync();

    public async Task<SchoolYear?> GetByIdAsync(Guid id) =>
        await _db.SchoolYears.FindAsync(id);

    public async Task<SchoolYear?> GetByYearAsync(int year) =>
        await _db.SchoolYears.FirstOrDefaultAsync(sy => sy.Year == year);

    public async Task<SchoolYear> CreateAsync(SchoolYear schoolYear)
    {
        _db.SchoolYears.Add(schoolYear);
        await _db.SaveChangesAsync();
        return schoolYear;
    }

    public async Task UpdateAsync(SchoolYear schoolYear) => await _db.SaveChangesAsync();
}

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly AppDbContext _db;
    public EnrollmentRepository(AppDbContext db) => _db = db;

    public async Task<Enrollment?> GetByIdAsync(Guid id) =>
        await _db.Enrollments.Include(e => e.Student).ThenInclude(s => s.User)
            .Include(e => e.Class).FirstOrDefaultAsync(e => e.Id == id);

    public async Task<List<Enrollment>> GetByStudentAsync(Guid studentId) =>
        await _db.Enrollments.Include(e => e.Class)
            .Where(e => e.StudentId == studentId).OrderByDescending(e => e.SchoolYear).ToListAsync();

    public async Task<List<Enrollment>> GetByClassAsync(Guid classId) =>
        await _db.Enrollments.Include(e => e.Student).ThenInclude(s => s.User)
            .Where(e => e.ClassId == classId).ToListAsync();

    public async Task<List<Enrollment>> GetAllAsync(int? year = null)
    {
        var query = _db.Enrollments.Include(e => e.Student).ThenInclude(s => s.User)
            .Include(e => e.Class).AsQueryable();
        if (year.HasValue) query = query.Where(e => e.SchoolYear == year.Value);
        return await query.OrderByDescending(e => e.SchoolYear).ThenBy(e => e.Student.User.Name).ToListAsync();
    }

    public async Task<Enrollment> CreateAsync(Enrollment enrollment)
    {
        _db.Enrollments.Add(enrollment);
        await _db.SaveChangesAsync();
        return enrollment;
    }

    public async Task UpdateAsync(Enrollment enrollment) => await _db.SaveChangesAsync();
}

public class InvoiceRepository : IInvoiceRepository
{
    private readonly AppDbContext _db;
    public InvoiceRepository(AppDbContext db) => _db = db;

    public async Task<Invoice?> GetByIdAsync(Guid id) => await _db.Invoices.FindAsync(id);

    public async Task<List<Invoice>> GetByStudentAsync(Guid studentId) =>
        await _db.Invoices.Where(i => i.StudentId == studentId).OrderByDescending(i => i.DueDate).ToListAsync();

    public async Task<Invoice> CreateAsync(Invoice invoice)
    {
        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();
        return invoice;
    }

    public async Task CreateBatchAsync(List<Invoice> invoices)
    {
        _db.Invoices.AddRange(invoices);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Invoice invoice) => await _db.SaveChangesAsync();
}

// === Permission ===
public class PermissionRepository : IPermissionRepository
{
    private readonly AppDbContext _db;
    public PermissionRepository(AppDbContext db) => _db = db;

    public async Task<List<Permission>> GetAllAsync() =>
        await _db.Permissions.OrderBy(p => p.Resource).ThenBy(p => p.Action).ToListAsync();

    public async Task<Permission?> GetByIdAsync(Guid id) =>
        await _db.Permissions.FindAsync(id);

    public async Task<Permission> CreateAsync(Permission permission)
    {
        _db.Permissions.Add(permission);
        await _db.SaveChangesAsync();
        return permission;
    }

    public async Task CreateBatchAsync(List<Permission> permissions)
    {
        _db.Permissions.AddRange(permissions);
        await _db.SaveChangesAsync();
    }
}

public class PermissionGroupRepository : IPermissionGroupRepository
{
    private readonly AppDbContext _db;
    public PermissionGroupRepository(AppDbContext db) => _db = db;

    public async Task<List<PermissionGroup>> GetByOrganizationAsync(Guid orgId) =>
        await _db.PermissionGroups.Include(pg => pg.GroupPermissions).ThenInclude(gp => gp.Permission)
            .Where(pg => pg.OrganizationId == orgId).OrderBy(pg => pg.Name).ToListAsync();

    public async Task<PermissionGroup?> GetByIdAsync(Guid id) =>
        await _db.PermissionGroups.Include(pg => pg.GroupPermissions).ThenInclude(gp => gp.Permission)
            .FirstOrDefaultAsync(pg => pg.Id == id);

    public async Task<PermissionGroup> CreateAsync(PermissionGroup group)
    {
        _db.PermissionGroups.Add(group);
        await _db.SaveChangesAsync();
        return group;
    }

    public async Task UpdateAsync(PermissionGroup group) => await _db.SaveChangesAsync();

    public async Task DeleteAsync(Guid id)
    {
        var group = await _db.PermissionGroups.FindAsync(id);
        if (group != null)
        {
            _db.PermissionGroups.Remove(group);
            await _db.SaveChangesAsync();
        }
    }
}

public class UserPermissionRepository : IUserPermissionRepository
{
    private readonly AppDbContext _db;
    public UserPermissionRepository(AppDbContext db) => _db = db;

    public async Task<List<UserPermission>> GetByUserAsync(Guid userId) =>
        await _db.UserPermissions.Include(up => up.Permission)
            .Where(up => up.UserId == userId).ToListAsync();

    public async Task SetPermissionAsync(Guid userId, Guid permissionId, bool granted)
    {
        var existing = await _db.UserPermissions
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId);
        if (existing != null)
        {
            existing.Granted = granted;
        }
        else
        {
            _db.UserPermissions.Add(new UserPermission
            {
                UserId = userId,
                PermissionId = permissionId,
                Granted = granted,
            });
        }
        await _db.SaveChangesAsync();
    }

    public async Task RemovePermissionAsync(Guid userId, Guid permissionId)
    {
        var existing = await _db.UserPermissions
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId);
        if (existing != null)
        {
            _db.UserPermissions.Remove(existing);
            await _db.SaveChangesAsync();
        }
    }
}

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _db;
    public CourseRepository(AppDbContext db) => _db = db;

    public async Task<Course?> GetByIdAsync(Guid id) =>
        await _db.Courses.Include(c => c.Classes).Include(c => c.Subjects)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<List<Course>> GetAllAsync(Guid orgId, bool activeOnly = true)
    {
        var query = _db.Courses.Where(c => c.OrganizationId == orgId).AsQueryable();
        if (activeOnly) query = query.Where(c => c.Active);
        return await query.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<Course> CreateAsync(Course course)
    {
        _db.Courses.Add(course);
        await _db.SaveChangesAsync();
        return course;
    }

    public async Task UpdateAsync(Course course)
    {
        course.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course != null)
        {
            course.Active = false;
            await _db.SaveChangesAsync();
        }
    }
}
public class StaffRepository : IStaffRepository
{
    private readonly AppDbContext _db;
    public StaffRepository(AppDbContext db) => _db = db;

    public async Task<Staff?> GetByIdAsync(Guid id) =>
        await _db.Staff.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == id);

    public async Task<List<Staff>> GetAllAsync(Guid orgId, bool activeOnly = true)
    {
        var query = _db.Staff.Include(s => s.User).Where(s => s.OrganizationId == orgId).AsQueryable();
        if (activeOnly) query = query.Where(s => s.Active);
        return await query.OrderBy(s => s.User.Name).ToListAsync();
    }

    public async Task<Staff> CreateAsync(Staff staff)
    {
        _db.Staff.Add(staff);
        await _db.SaveChangesAsync();
        return staff;
    }

    public async Task UpdateAsync(Staff staff) => await _db.SaveChangesAsync();
}

public class LessonPlanRepository : ILessonPlanRepository
{
    private readonly AppDbContext _db;
    public LessonPlanRepository(AppDbContext db) => _db = db;

    public async Task<LessonPlan?> GetByIdAsync(Guid id) =>
        await _db.LessonPlans.Include(l => l.Class).Include(l => l.Subject).Include(l => l.Teacher).ThenInclude(t => t.User)
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task<List<LessonPlan>> GetByClassAndSubjectAsync(Guid classId, Guid subjectId, DateTime? from, DateTime? to)
    {
        var query = _db.LessonPlans.Include(l => l.Class).Include(l => l.Subject).Include(l => l.Teacher).ThenInclude(t => t.User)
            .Where(l => l.ClassId == classId && l.SubjectId == subjectId).AsQueryable();
        if (from.HasValue) query = query.Where(l => l.Date >= from.Value);
        if (to.HasValue) query = query.Where(l => l.Date <= to.Value);
        return await query.OrderBy(l => l.Date).ToListAsync();
    }

    public async Task<List<LessonPlan>> GetByTeacherAsync(Guid teacherId, DateTime? from, DateTime? to)
    {
        var query = _db.LessonPlans.Include(l => l.Class).Include(l => l.Subject).Include(l => l.Teacher).ThenInclude(t => t.User)
            .Where(l => l.TeacherId == teacherId).AsQueryable();
        if (from.HasValue) query = query.Where(l => l.Date >= from.Value);
        if (to.HasValue) query = query.Where(l => l.Date <= to.Value);
        return await query.OrderBy(l => l.Date).ToListAsync();
    }

    public async Task<LessonPlan> CreateAsync(LessonPlan plan) { _db.LessonPlans.Add(plan); await _db.SaveChangesAsync(); return plan; }
    public async Task UpdateAsync(LessonPlan plan) { plan.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(); }
    public async Task DeleteAsync(Guid id) { var p = await _db.LessonPlans.FindAsync(id); if (p != null) { _db.LessonPlans.Remove(p); await _db.SaveChangesAsync(); } }
}

public class AssessmentRepository : IAssessmentRepository
{
    private readonly AppDbContext _db;
    public AssessmentRepository(AppDbContext db) => _db = db;

    public async Task<Assessment?> GetByIdAsync(Guid id) =>
        await _db.Assessments.Include(a => a.Subject).Include(a => a.Class).FirstOrDefaultAsync(a => a.Id == id);

    public async Task<List<Assessment>> GetByClassAndBimesterAsync(Guid classId, int bimester, int year) =>
        await _db.Assessments.Include(a => a.Subject).Include(a => a.Class)
            .Where(a => a.ClassId == classId && a.Bimester == bimester && a.SchoolYear == year)
            .OrderBy(a => a.Date).ToListAsync();

    public async Task<List<Assessment>> GetBySubjectAndClassAsync(Guid subjectId, Guid classId, int year) =>
        await _db.Assessments.Include(a => a.Subject)
            .Where(a => a.SubjectId == subjectId && a.ClassId == classId && a.SchoolYear == year)
            .OrderBy(a => a.Bimester).ThenBy(a => a.Date).ToListAsync();

    public async Task<Assessment> CreateAsync(Assessment a) { _db.Assessments.Add(a); await _db.SaveChangesAsync(); return a; }
    public async Task UpdateAsync(Assessment a) { await _db.SaveChangesAsync(); }
    public async Task DeleteAsync(Guid id) { var a = await _db.Assessments.FindAsync(id); if (a != null) { _db.Assessments.Remove(a); await _db.SaveChangesAsync(); } }
}

public class AssessmentGradeRepository : IAssessmentGradeRepository
{
    private readonly AppDbContext _db;
    public AssessmentGradeRepository(AppDbContext db) => _db = db;

    public async Task<AssessmentGrade?> GetByIdAsync(Guid id) => await _db.AssessmentGrades.FindAsync(id);

    public async Task<List<AssessmentGrade>> GetByAssessmentAsync(Guid assessmentId) =>
        await _db.AssessmentGrades.Include(ag => ag.Student).ThenInclude(s => s.User)
            .Where(ag => ag.AssessmentId == assessmentId).OrderBy(ag => ag.Student.User.Name).ToListAsync();

    public async Task<List<AssessmentGrade>> GetByStudentAndBimesterAsync(Guid studentId, int bimester, int year) =>
        await _db.AssessmentGrades.Include(ag => ag.Assessment)
            .Where(ag => ag.StudentId == studentId && ag.Assessment.Bimester == bimester && ag.Assessment.SchoolYear == year)
            .ToListAsync();

    public async Task<AssessmentGrade> CreateAsync(AssessmentGrade g) { _db.AssessmentGrades.Add(g); await _db.SaveChangesAsync(); return g; }

    public async Task CreateBatchAsync(List<AssessmentGrade> grades) { _db.AssessmentGrades.AddRange(grades); await _db.SaveChangesAsync(); }
    public async Task UpdateAsync(AssessmentGrade g) { await _db.SaveChangesAsync(); }
}

public class ScheduleEntryRepository : IScheduleEntryRepository
{
    private readonly AppDbContext _db;
    public ScheduleEntryRepository(AppDbContext db) => _db = db;

    public async Task<ScheduleEntry?> GetByIdAsync(Guid id) =>
        await _db.ScheduleEntries.Include(s => s.Class).Include(s => s.Subject).Include(s => s.Teacher).ThenInclude(t => t.User)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<List<ScheduleEntry>> GetByClassAsync(Guid classId) =>
        await _db.ScheduleEntries.Include(s => s.Subject).Include(s => s.Teacher).ThenInclude(t => t.User)
            .Where(s => s.ClassId == classId).OrderBy(s => s.DayOfWeek).ThenBy(s => s.StartTime).ToListAsync();

    public async Task<List<ScheduleEntry>> GetByTeacherAsync(Guid teacherId) =>
        await _db.ScheduleEntries.Include(s => s.Class).Include(s => s.Subject).ThenInclude(t => t.User)
            .Where(s => s.TeacherId == teacherId).OrderBy(s => s.DayOfWeek).ThenBy(s => s.StartTime).ToListAsync();

    public async Task<ScheduleEntry> CreateAsync(ScheduleEntry e) { _db.ScheduleEntries.Add(e); await _db.SaveChangesAsync(); return e; }
    public async Task UpdateAsync(ScheduleEntry e) { await _db.SaveChangesAsync(); }
    public async Task DeleteAsync(Guid id) { var e = await _db.ScheduleEntries.FindAsync(id); if (e != null) { _db.ScheduleEntries.Remove(e); await _db.SaveChangesAsync(); } }
}

public class GradingRuleRepository : IGradingRuleRepository
{
    private readonly AppDbContext _db;
    public GradingRuleRepository(AppDbContext db) => _db = db;

    public async Task<GradingRule?> GetByIdAsync(Guid id) => await _db.GradingRules.FindAsync(id);
    public async Task<GradingRule?> GetByOrganizationAsync(Guid orgId) =>
        await _db.GradingRules.FirstOrDefaultAsync(g => g.OrganizationId == orgId && g.Active);
    public async Task<GradingRule> CreateAsync(GradingRule r) { _db.GradingRules.Add(r); await _db.SaveChangesAsync(); return r; }
    public async Task UpdateAsync(GradingRule r) { await _db.SaveChangesAsync(); }
}
}
}
