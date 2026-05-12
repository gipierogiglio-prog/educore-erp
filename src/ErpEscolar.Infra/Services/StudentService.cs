using ErpEscolar.Core.Entities;
using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;

namespace ErpEscolar.Infra.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepo;
    private readonly IUserRepository _userRepo;

    public StudentService(IStudentRepository studentRepo, IUserRepository userRepo)
    {
        _studentRepo = studentRepo;
        _userRepo = userRepo;
    }

    public async Task<List<StudentResponse>> GetAllAsync()
    {
        var students = await _studentRepo.GetAllAsync();
        return students.Select(s => new StudentResponse(
            s.Id, s.User.Name, s.User.Email, s.Enrollment,
            s.Class?.Name, s.Active ? "Ativo" : "Inativo",
            s.Guardian?.User?.Name
        )).ToList();
    }

    public async Task<StudentResponse?> GetByIdAsync(Guid id)
    {
        var s = await _studentRepo.GetByIdAsync(id);
        if (s == null) return null;
        return new StudentResponse(s.Id, s.User.Name, s.User.Email, s.Enrollment,
            s.Class?.Name, s.Active ? "Ativo" : "Inativo", s.Guardian?.User?.Name);
    }

    public async Task<StudentResponse> CreateAsync(CreateStudentRequest request)
    {
        var existingUser = await _userRepo.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new InvalidOperationException("Email já cadastrado");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "student",
            Phone = request.Phone,
        };

        user = await _userRepo.CreateAsync(user);

        // Create guardian if data provided
        Guardian? guardian = null;
        if (!string.IsNullOrEmpty(request.GuardianName))
        {
            var guardianUser = new User
            {
                Name = request.GuardianName,
                Email = $"resp_{user.Id}@escola.com", // placeholder email
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Role = "guardian",
                Phone = request.GuardianPhone,
            };
            guardianUser = await _userRepo.CreateAsync(guardianUser);
        }

        // Generate enrollment number
        var enrollment = $"ALU{DateTime.UtcNow:yyyyMMdd}{user.Id.ToString()[..4].ToUpper()}";

        var student = new Student
        {
            UserId = user.Id,
            Enrollment = enrollment,
            ClassId = request.ClassId,
            GuardianId = guardian?.Id,
        };

        student = await _studentRepo.CreateAsync(student);
        return new StudentResponse(student.Id, user.Name, user.Email, enrollment,
            null, "Ativo", guardian?.User?.Name);
    }

    public async Task<bool> ToggleStatusAsync(Guid id)
    {
        var student = await _studentRepo.GetByIdAsync(id);
        if (student == null) return false;
        student.Active = !student.Active;
        await _studentRepo.UpdateAsync(student);
        return student.Active;
    }
}
