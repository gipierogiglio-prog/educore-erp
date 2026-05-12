using ErpEscolar.Core.Entities;

namespace ErpEscolar.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task UpdateAsync(User user);
}

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(Guid id);
    Task<List<Student>> GetAllAsync(bool activeOnly = true);
    Task<List<Student>> GetByClassIdAsync(Guid classId);
    Task<Student> CreateAsync(Student student);
    Task UpdateAsync(Student student);
}

public interface ITeacherRepository
{
    Task<Teacher?> GetByIdAsync(Guid id);
    Task<List<Teacher>> GetAllAsync(bool activeOnly = true);
    Task<Teacher> CreateAsync(Teacher teacher);
    Task UpdateAsync(Teacher teacher);
}

public interface IClassRepository
{
    Task<Class?> GetByIdAsync(Guid id);
    Task<List<Class>> GetAllAsync(int? year = null);
    Task<Class> CreateAsync(Class classEntity);
    Task UpdateAsync(Class classEntity);
}

public interface ISubjectRepository
{
    Task<Subject?> GetByIdAsync(Guid id);
    Task<List<Subject>> GetAllAsync();
    Task<Subject> CreateAsync(Subject subject);
}

public interface IGradeRepository
{
    Task<Grade?> GetByIdAsync(Guid id);
    Task<List<Grade>> GetByStudentAsync(Guid studentId);
    Task<List<Grade>> GetByClassAndBimesterAsync(Guid classId, int bimester, int year);
    Task<Grade> CreateAsync(Grade grade);
    Task CreateBatchAsync(List<Grade> grades);
    Task UpdateAsync(Grade grade);
}

public interface IAttendanceRepository
{
    Task<Attendance?> GetByIdAsync(Guid id);
    Task<List<Attendance>> GetByClassAndDateAsync(Guid classId, DateTime date);
    Task CreateBatchAsync(List<Attendance> attendances);
}

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id);
    Task<List<Invoice>> GetByStudentAsync(Guid studentId);
    Task<Invoice> CreateAsync(Invoice invoice);
    Task CreateBatchAsync(List<Invoice> invoices);
    Task UpdateAsync(Invoice invoice);
}
