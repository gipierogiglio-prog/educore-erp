using ErpEscolar.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpEscolar.Infra.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Guardian> Guardians => Set<Guardian>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<TeacherSubject> TeacherSubjects => Set<TeacherSubject>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<TuitionPlan> TuitionPlans => Set<TuitionPlan>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<SchoolYear> SchoolYears => Set<SchoolYear>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Staff> Staff => Set<Staff>();

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Staff> Staff => Set<Staff>();
    public DbSet<LessonPlan> LessonPlans => Set<LessonPlan>();
    public DbSet<Assessment> Assessments => Set<Assessment>();
    public DbSet<AssessmentGrade> AssessmentGrades => Set<AssessmentGrade>();
    public DbSet<ScheduleEntry> ScheduleEntries => Set<ScheduleEntry>();
    public DbSet<GradingRule> GradingRules => Set<GradingRule>();
    public DbSet<PermissionGroup> PermissionGroups => Set<PermissionGroup>();
    public DbSet<GroupPermission> GroupPermissions => Set<GroupPermission>();
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
    public DbSet<UserGroup> UserGroups => Set<UserGroup>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Staff>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Position).HasMaxLength(100).IsRequired();
            e.Property(s => s.Department).HasMaxLength(50);
            e.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId);
        });

        modelBuilder.Entity<Course>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).HasMaxLength(200).IsRequired();
            e.HasMany(c => c.Classes).WithOne(c => c.Course).HasForeignKey(c => c.CourseId).OnDelete(DeleteBehavior.SetNull);
            e.HasMany(c => c.Subjects).WithOne(s => s.Course).HasForeignKey(s => s.CourseId).OnDelete(DeleteBehavior.SetNull);
        });

    {
        // User

        modelBuilder.Entity<Staff>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Position).HasMaxLength(100).IsRequired();
            e.Property(s => s.Department).HasMaxLength(50);
            e.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId);
        });

        modelBuilder.Entity<Course>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).HasMaxLength(200).IsRequired();
            e.HasMany(c => c.Classes).WithOne(c => c.Course).HasForeignKey(c => c.CourseId).OnDelete(DeleteBehavior.SetNull);
            e.HasMany(c => c.Subjects).WithOne(s => s.Course).HasForeignKey(s => s.CourseId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<LessonPlan>(e =>
        {
            e.HasKey(l => l.Id);
            e.Property(l => l.Topic).HasMaxLength(255);
            e.HasOne(l => l.Class).WithMany().HasForeignKey(l => l.ClassId);
            e.HasOne(l => l.Subject).WithMany().HasForeignKey(l => l.SubjectId);
            e.HasOne(l => l.Teacher).WithMany().HasForeignKey(l => l.TeacherId);
        });

        modelBuilder.Entity<Assessment>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.Title).HasMaxLength(255).IsRequired();
            e.Property(a => a.Type).HasMaxLength(20).IsRequired();
            e.HasOne(a => a.Subject).WithMany().HasForeignKey(a => a.SubjectId);
            e.HasOne(a => a.Class).WithMany().HasForeignKey(a => a.ClassId);
        });

        modelBuilder.Entity<AssessmentGrade>(e =>
        {
            e.HasKey(ag => ag.Id);
            e.HasOne(ag => ag.Assessment).WithMany().HasForeignKey(ag => ag.AssessmentId);
            e.HasOne(ag => ag.Student).WithMany().HasForeignKey(ag => ag.StudentId);
        });

        modelBuilder.Entity<ScheduleEntry>(e =>
        {
            e.HasKey(s => s.Id);
            e.HasOne(s => s.Class).WithMany().HasForeignKey(s => s.ClassId);
            e.HasOne(s => s.Subject).WithMany().HasForeignKey(s => s.SubjectId);
            e.HasOne(s => s.Teacher).WithMany().HasForeignKey(s => s.TeacherId);
        });

        modelBuilder.Entity<GradingRule>(e =>
        {
            e.HasKey(g => g.Id);
            e.Property(g => g.Name).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Name).HasMaxLength(200).IsRequired();
            e.Property(u => u.Email).HasMaxLength(200).IsRequired();
            e.Property(u => u.Role).HasMaxLength(20).IsRequired();
        });

        // Student
        modelBuilder.Entity<Student>(e =>
        {
            e.HasKey(s => s.Id);
            e.HasIndex(s => s.Enrollment).IsUnique();
            e.Property(s => s.Enrollment).HasMaxLength(20).IsRequired();
            e.HasOne(s => s.User).WithOne(u => u.Student).HasForeignKey<Student>(s => s.UserId);
            e.HasOne(s => s.Class).WithMany(c => c.Students).HasForeignKey(s => s.ClassId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(s => s.Guardian).WithMany(g => g.Students).HasForeignKey(s => s.GuardianId).OnDelete(DeleteBehavior.SetNull);
        });

        // Teacher
        modelBuilder.Entity<Teacher>(e =>
        {
            e.HasKey(t => t.Id);
            e.HasOne(t => t.User).WithOne(u => u.Teacher).HasForeignKey<Teacher>(t => t.UserId);
        });

        // Class
        modelBuilder.Entity<Class>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).HasMaxLength(100).IsRequired();
            e.Property(c => c.Shift).HasMaxLength(20).IsRequired();
        });

        // Subject
        modelBuilder.Entity<Subject>(e =>
        {
            e.HasKey(s => s.Id);
            e.HasIndex(s => s.Code).IsUnique();
            e.Property(s => s.Code).HasMaxLength(20).IsRequired();
        });

        // TeacherSubject (many-to-many with extra data)
        modelBuilder.Entity<TeacherSubject>(e =>
        {
            e.HasKey(ts => ts.Id);
            e.HasIndex(ts => new { ts.TeacherId, ts.SubjectId, ts.ClassId }).IsUnique();
            e.HasOne(ts => ts.Teacher).WithMany(t => t.TeacherSubjects).HasForeignKey(ts => ts.TeacherId);
            e.HasOne(ts => ts.Subject).WithMany(s => s.TeacherSubjects).HasForeignKey(ts => ts.SubjectId);
            e.HasOne(ts => ts.Class).WithMany(c => c.TeacherSubjects).HasForeignKey(ts => ts.ClassId);
        });

        // Grade
        modelBuilder.Entity<Grade>(e =>
        {
            e.HasKey(g => g.Id);
            e.HasIndex(g => new { g.StudentId, g.SubjectId, g.Bimester, g.SchoolYear }).IsUnique();
            e.Property(g => g.Value).HasColumnType("decimal(5,2)");
            e.Property(g => g.RecoveryValue).HasColumnType("decimal(5,2)");
        });

        // Attendance
        modelBuilder.Entity<Attendance>(e =>
        {
            e.HasKey(a => a.Id);
            e.HasIndex(a => new { a.StudentId, a.ClassId, a.Date });
        });

        // Invoice
        modelBuilder.Entity<Invoice>(e =>
        {
            e.HasKey(i => i.Id);
            e.Property(i => i.Value).HasColumnType("decimal(10,2)");
            e.Property(i => i.Status).HasMaxLength(20).IsRequired();
        });

        // Organization
        modelBuilder.Entity<Organization>(e =>
        {
            e.HasKey(o => o.Id);
            e.HasIndex(o => o.Slug).IsUnique();
            e.Property(o => o.Status).HasMaxLength(20).IsRequired();
        });

        // User -> Organization

        modelBuilder.Entity<Staff>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Position).HasMaxLength(100).IsRequired();
            e.Property(s => s.Department).HasMaxLength(50);
            e.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId);
        });

        modelBuilder.Entity<Course>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).HasMaxLength(200).IsRequired();
            e.HasMany(c => c.Classes).WithOne(c => c.Course).HasForeignKey(c => c.CourseId).OnDelete(DeleteBehavior.SetNull);
            e.HasMany(c => c.Subjects).WithOne(s => s.Course).HasForeignKey(s => s.CourseId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<LessonPlan>(e =>
        {
            e.HasKey(l => l.Id);
            e.Property(l => l.Topic).HasMaxLength(255);
            e.HasOne(l => l.Class).WithMany().HasForeignKey(l => l.ClassId);
            e.HasOne(l => l.Subject).WithMany().HasForeignKey(l => l.SubjectId);
            e.HasOne(l => l.Teacher).WithMany().HasForeignKey(l => l.TeacherId);
        });

        modelBuilder.Entity<Assessment>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.Title).HasMaxLength(255).IsRequired();
            e.Property(a => a.Type).HasMaxLength(20).IsRequired();
            e.HasOne(a => a.Subject).WithMany().HasForeignKey(a => a.SubjectId);
            e.HasOne(a => a.Class).WithMany().HasForeignKey(a => a.ClassId);
        });

        modelBuilder.Entity<AssessmentGrade>(e =>
        {
            e.HasKey(ag => ag.Id);
            e.HasOne(ag => ag.Assessment).WithMany().HasForeignKey(ag => ag.AssessmentId);
            e.HasOne(ag => ag.Student).WithMany().HasForeignKey(ag => ag.StudentId);
        });

        modelBuilder.Entity<ScheduleEntry>(e =>
        {
            e.HasKey(s => s.Id);
            e.HasOne(s => s.Class).WithMany().HasForeignKey(s => s.ClassId);
            e.HasOne(s => s.Subject).WithMany().HasForeignKey(s => s.SubjectId);
            e.HasOne(s => s.Teacher).WithMany().HasForeignKey(s => s.TeacherId);
        });

        modelBuilder.Entity<GradingRule>(e =>
        {
            e.HasKey(g => g.Id);
            e.Property(g => g.Name).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<User>(e =>
        {
            e.HasOne(u => u.Organization).WithMany().HasForeignKey(u => u.OrganizationId).OnDelete(DeleteBehavior.SetNull);
        });

        // SchoolYear
        modelBuilder.Entity<SchoolYear>(e =>
        {
            e.HasKey(sy => sy.Id);
            e.HasIndex(sy => sy.Year).IsUnique();
            e.Property(sy => sy.Status).HasMaxLength(20).IsRequired();
        });

        // Enrollment
        modelBuilder.Entity<Enrollment>(e =>
        {
            e.HasKey(en => en.Id);
            e.HasIndex(en => new { en.StudentId, en.ClassId, en.SchoolYear }).IsUnique();
            e.Property(en => en.Status).HasMaxLength(20).IsRequired();
            e.HasOne(en => en.Student).WithMany(s => s.Enrollments).HasForeignKey(en => en.StudentId);
            e.HasOne(en => en.Class).WithMany(c => c.Enrollments).HasForeignKey(en => en.ClassId);
        });

        // TuitionPlan
        modelBuilder.Entity<TuitionPlan>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Value).HasColumnType("decimal(10,2)");
        });

        // Permission
        modelBuilder.Entity<Permission>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasIndex(p => new { p.Resource, p.Action }).IsUnique();
            e.Property(p => p.Resource).HasMaxLength(50).IsRequired();
            e.Property(p => p.Action).HasMaxLength(50).IsRequired();
        });

        // PermissionGroup
        modelBuilder.Entity<PermissionGroup>(e =>
        {
            e.HasKey(pg => pg.Id);
            e.Property(pg => pg.Name).HasMaxLength(100).IsRequired();
            e.HasOne(pg => pg.Organization).WithMany().HasForeignKey(pg => pg.OrganizationId);
        });

        // GroupPermission
        modelBuilder.Entity<GroupPermission>(e =>
        {
            e.HasKey(gp => gp.Id);
            e.HasIndex(gp => new { gp.GroupId, gp.PermissionId }).IsUnique();
            e.HasOne(gp => gp.Group).WithMany(g => g.GroupPermissions).HasForeignKey(gp => gp.GroupId);
            e.HasOne(gp => gp.Permission).WithMany().HasForeignKey(gp => gp.PermissionId);
        });

        // UserPermission
        modelBuilder.Entity<UserPermission>(e =>
        {
            e.HasKey(up => up.Id);
            e.HasIndex(up => new { up.UserId, up.PermissionId }).IsUnique();
            e.HasOne(up => up.User).WithMany().HasForeignKey(up => up.UserId);
            e.HasOne(up => up.Permission).WithMany().HasForeignKey(up => up.PermissionId);
        });

        // UserGroup
        modelBuilder.Entity<UserGroup>(e =>
        {
            e.HasKey(ug => ug.Id);
            e.HasIndex(ug => new { ug.UserId, ug.GroupId }).IsUnique();
            e.HasOne(ug => ug.User).WithMany().HasForeignKey(ug => ug.UserId);
            e.HasOne(ug => ug.Group).WithMany(g => g.UserGroups).HasForeignKey(ug => ug.GroupId);
        });
    }
}
