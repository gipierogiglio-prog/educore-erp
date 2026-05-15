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
    Task<List<Student>> GetAllAsync(Guid orgId, bool activeOnly = true);
    Task<List<Student>> GetByClassIdAsync(Guid classId);
    Task<Student> CreateAsync(Student student);
    Task UpdateAsync(Student student);
}

public interface ITeacherRepository
{
    Task<Teacher?> GetByIdAsync(Guid id);
    Task<List<Teacher>> GetAllAsync(bool activeOnly = true, Guid? orgId = null);
    Task<Teacher> CreateAsync(Teacher teacher);
    Task UpdateAsync(Teacher teacher);
}

public interface IClassRepository
{
    Task<Class?> GetByIdAsync(Guid id);
    Task<List<Class>> GetAllAsync(int? year = null, Guid? orgId = null);
    Task<Class> CreateAsync(Class classEntity);
    Task UpdateAsync(Class classEntity);
}

public interface ISubjectRepository
{
    Task<Subject?> GetByIdAsync(Guid id);
    Task<List<Subject>> GetAllAsync(Guid? orgId = null);
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

public interface ISchoolYearRepository
{
    Task<List<SchoolYear>> GetAllAsync();
    Task<SchoolYear?> GetByIdAsync(Guid id);
    Task<SchoolYear?> GetByYearAsync(int year);
    Task<SchoolYear> CreateAsync(SchoolYear schoolYear);
    Task UpdateAsync(SchoolYear schoolYear);
}

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetByIdAsync(Guid id);
    Task<List<Enrollment>> GetByStudentAsync(Guid studentId);
    Task<List<Enrollment>> GetByClassAsync(Guid classId);
    Task<List<Enrollment>> GetAllAsync(int? year = null);
    Task<Enrollment> CreateAsync(Enrollment enrollment);
    Task UpdateAsync(Enrollment enrollment);
}

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id);
    Task<List<Invoice>> GetByStudentAsync(Guid studentId);
    Task<Invoice> CreateAsync(Invoice invoice);
    Task CreateBatchAsync(List<Invoice> invoices);
    Task UpdateAsync(Invoice invoice);
}

// === Permissions ===
public interface IPermissionRepository
{
    Task<List<Permission>> GetAllAsync();
    Task<Permission?> GetByIdAsync(Guid id);
    Task<Permission> CreateAsync(Permission permission);
    Task CreateBatchAsync(List<Permission> permissions);
}

public interface IPermissionGroupRepository
{
    Task<List<PermissionGroup>> GetByOrganizationAsync(Guid orgId);
    Task<PermissionGroup?> GetByIdAsync(Guid id);
    Task<PermissionGroup> CreateAsync(PermissionGroup group);
    Task UpdateAsync(PermissionGroup group);
    Task DeleteAsync(Guid id);
}

public interface IUserPermissionRepository
{
    Task<List<UserPermission>> GetByUserAsync(Guid userId);
    Task SetPermissionAsync(Guid userId, Guid permissionId, bool granted);
    Task RemovePermissionAsync(Guid userId, Guid permissionId);
}

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid id);
    Task<List<Course>> GetAllAsync(Guid orgId, bool activeOnly = true);
    Task<Course> CreateAsync(Course course);
    Task UpdateAsync(Course course);
    Task DeleteAsync(Guid id);
}
