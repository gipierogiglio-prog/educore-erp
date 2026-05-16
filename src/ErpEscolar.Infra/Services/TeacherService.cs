using ErpEscolar.Core.Entities;
using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;

namespace ErpEscolar.Infra.Services;

public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _teacherRepo;
    private readonly IUserRepository _userRepo;

    public TeacherService(ITeacherRepository teacherRepo, IUserRepository userRepo)
    {
        _teacherRepo = teacherRepo;
        _userRepo = userRepo;
    }

    public async Task<List<TeacherResponse>> GetAllAsync(Guid orgId)
    {
        var teachers = await _teacherRepo.GetAllAsync(orgId: orgId);
        return teachers.Select(t => new TeacherResponse(
            t.Id, t.User.Name, t.User.Email,
            t.Specialization, t.TeacherSubjects.Count
        )).ToList();
    }

    public async Task<TeacherResponse?> GetByIdAsync(Guid id)
    {
        var t = await _teacherRepo.GetByIdAsync(id);
        if (t == null) return null;
        return new TeacherResponse(t.Id, t.User.Name, t.User.Email,
            t.Specialization, t.TeacherSubjects.Count);
    }

    public async Task<TeacherResponse> CreateAsync(CreateTeacherRequest request, Guid orgId)
    {
        var existingUser = await _userRepo.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new InvalidOperationException("Email já cadastrado");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "teacher",
            Phone = request.Phone,
        };

        user = await _userRepo.CreateAsync(user);

        var teacher = new Teacher
        {
            UserId = user.Id,
            OrganizationId = orgId,
            Specialization = request.Specialization,
        };

        teacher = await _teacherRepo.CreateAsync(teacher);
        return new TeacherResponse(teacher.Id, user.Name, user.Email,
            teacher.Specialization, 0);
    }
}
