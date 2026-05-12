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

    public async Task<List<Student>> GetAllAsync(bool activeOnly = true)
    {
        var query = _db.Students.Include(s => s.User).Include(s => s.Class).AsQueryable();
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

    public async Task<List<Teacher>> GetAllAsync(bool activeOnly = true)
    {
        var query = _db.Teachers.Include(t => t.User).Include(t => t.TeacherSubjects).AsQueryable();
        if (activeOnly) query = query.Where(t => t.Active);
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

    public async Task<List<Class>> GetAllAsync(int? year = null)
    {
        var query = _db.Classes.Include(c => c.Students).AsQueryable();
        if (year.HasValue) query = query.Where(c => c.Year == year.Value);
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
    public async Task<List<Subject>> GetAllAsync() => await _db.Subjects.OrderBy(s => s.Name).ToListAsync();

    public async Task<Subject> CreateAsync(Subject subject)
    {
        _db.Subjects.Add(subject);
        await _db.SaveChangesAsync();
        return subject;
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
