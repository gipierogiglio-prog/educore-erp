using ErpEscolar.Core.Entities;
using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;

namespace ErpEscolar.Infra.Services;

public class AssessmentService : IAssessmentService
{
    private readonly IAssessmentRepository _repo;
    private readonly IAssessmentGradeRepository _gradeRepo;
    private readonly IStudentRepository _studentRepo;

    public AssessmentService(IAssessmentRepository repo, IAssessmentGradeRepository gradeRepo, IStudentRepository studentRepo)
    {
        _repo = repo;
        _gradeRepo = gradeRepo;
        _studentRepo = studentRepo;
    }

    public async Task<List<AssessmentResponse>> GetByClassAsync(Guid classId, int? bimester, int? year)
    {
        var assessments = await _repo.GetByClassAndBimesterAsync(classId, bimester ?? 1, year ?? DateTime.UtcNow.Year);
        return assessments.Select(a => new AssessmentResponse(
            a.Id, a.Title, a.Type, a.SubjectId, a.Subject?.Name ?? "",
            a.ClassId, a.Class?.Name ?? "", a.Bimester, a.SchoolYear,
            a.Date, a.MaxScore, a.Weight, a.Description
        )).ToList();
    }

    public async Task<AssessmentResponse> CreateAsync(CreateAssessmentRequest request, Guid orgId)
    {
        var assessment = new Assessment
        {
            Title = request.Title, Type = request.Type,
            SubjectId = request.SubjectId, ClassId = request.ClassId,
            Bimester = request.Bimester, SchoolYear = request.Year,
            Date = request.Date, MaxScore = request.MaxScore,
            Weight = request.Weight, Description = request.Description,
            OrganizationId = orgId
        };
        assessment = await _repo.CreateAsync(assessment);
        return new AssessmentResponse(assessment.Id, assessment.Title, assessment.Type,
            assessment.SubjectId, "", assessment.ClassId, "",
            assessment.Bimester, assessment.SchoolYear, assessment.Date,
            assessment.MaxScore, assessment.Weight, assessment.Description);
    }

    public async Task SubmitGradesAsync(Guid assessmentId, List<SubmitGradeItem> grades)
    {
        var assessment = await _repo.GetByIdAsync(assessmentId);
        if (assessment == null) throw new KeyNotFoundException("Avaliacao nao encontrada");

        var gradeEntities = grades.Select(g => new AssessmentGrade
        {
            AssessmentId = assessmentId, StudentId = g.StudentId,
            Score = Math.Min(g.Score, assessment.MaxScore), Notes = g.Notes
        }).ToList();

        await _gradeRepo.CreateBatchAsync(gradeEntities);
    }

    public async Task<List<AssessmentGradeResponse>> GetGradesAsync(Guid assessmentId)
    {
        var grades = await _gradeRepo.GetByAssessmentAsync(assessmentId);
        return grades.Select(g => new AssessmentGradeResponse(
            g.Id, g.StudentId, g.Student?.User?.Name ?? "", g.Score, g.Notes
        )).ToList();
    }
}
