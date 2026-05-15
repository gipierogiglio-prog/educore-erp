using ErpEscolar.Core.Entities;
using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;

namespace ErpEscolar.Infra.Services;
using ErpEscolar.Core.Entities;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _repo;
    private readonly IStudentRepository _studentRepo;
    private readonly IClassRepository _classRepo;

    public EnrollmentService(IEnrollmentRepository repo, IStudentRepository studentRepo, IClassRepository classRepo)
    {
        _repo = repo;
        _studentRepo = studentRepo;
        _classRepo = classRepo;
    }

    public async Task<List<EnrollmentResponse>> GetAllAsync(int? year = null)
    {
        var enrollments = await _repo.GetAllAsync(year);
        return enrollments.Select(MapToResponse).ToList();
    }

    public async Task<List<EnrollmentResponse>> GetByStudentAsync(Guid studentId)
    {
        var enrollments = await _repo.GetByStudentAsync(studentId);
        return enrollments.Select(MapToResponse).ToList();
    }

    public async Task<EnrollmentResponse> CreateAsync(CreateEnrollmentRequest request)
    {
        var student = await _studentRepo.GetByIdAsync(request.StudentId);
        if (student == null) throw new KeyNotFoundException("Aluno não encontrado");

        var classEntity = await _classRepo.GetByIdAsync(request.ClassId);
        if (classEntity == null) throw new KeyNotFoundException("Turma não encontrada");

        var enrollment = new Enrollment
        {
            StudentId = request.StudentId,
            ClassId = request.ClassId,
            SchoolYear = request.SchoolYear ?? DateTime.UtcNow.Year,
            Notes = request.Notes,
        };

        enrollment = await _repo.CreateAsync(enrollment);
        return MapToResponse(enrollment);
    }

    public async Task UpdateStatusAsync(Guid id, string status)
    {
        var enrollment = await _repo.GetByIdAsync(id);
        if (enrollment == null) throw new KeyNotFoundException("Matrícula não encontrada");

        enrollment.Status = status;
        if (status != "active") enrollment.EndDate = DateTime.UtcNow;
        await _repo.UpdateAsync(enrollment);
    }

    private static EnrollmentResponse MapToResponse(Enrollment e) => new(
        e.Id, e.StudentId, e.Student?.User?.Name ?? "", e.Student?.Enrollment ?? "",
        e.ClassId, e.Class?.Name ?? "", e.SchoolYear, e.Status, e.EnrollmentDate
    );

    public async Task<TransferResponse> TransferAsync(TransferRequest request, Guid orgId)
    {
        var student = await _studentRepo.GetByIdAsync(request.StudentId);
        if (student == null) throw new KeyNotFoundException("Aluno nao encontrado");

        var fromClass = await _classRepo.GetByIdAsync(request.FromClassId);
        var toClass = await _classRepo.GetByIdAsync(request.ToClassId);
        if (fromClass == null || toClass == null)
            throw new KeyNotFoundException("Turma nao encontrada");

        var oldEnrollments = await _repo.GetByStudentAsync(request.StudentId);
        var activeEnrollment = oldEnrollments.FirstOrDefault(e => e.Status == "active");
        if (activeEnrollment != null)
        {
            activeEnrollment.Status = "transferred";
            activeEnrollment.EndDate = DateTime.UtcNow;
            await _repo.UpdateAsync(activeEnrollment);
        }

        student.ClassId = request.ToClassId;
        await _studentRepo.UpdateAsync(student);

        var newEnrollment = new Enrollment
        {
            StudentId = request.StudentId,
            ClassId = request.ToClassId,
            SchoolYear = DateTime.UtcNow.Year,
            OrganizationId = orgId,
            Status = "active",
            Notes = $"Transferido de {fromClass.Name}. Motivo: {request.Reason}"
        };
        newEnrollment = await _repo.CreateAsync(newEnrollment);

        return new TransferResponse(
            newEnrollment.Id, student.User.Name, fromClass.Name, toClass.Name,
            DateTime.UtcNow, request.Reason, "active"
        );
    }
}