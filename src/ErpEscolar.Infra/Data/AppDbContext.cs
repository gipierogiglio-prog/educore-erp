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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User
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
    }
}
