using ErpEscolar.Core.Entities;
using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;

namespace ErpEscolar.Infra.Services;

public class LessonPlanService : ILessonPlanService
{
    private readonly ILessonPlanRepository _repo;

    public LessonPlanService(ILessonPlanRepository repo) => _repo = repo;

    public async Task<List<LessonPlanResponse>> GetByClassAsync(Guid classId, Guid? subjectId, int? month, int? year)
    {
        var from = month.HasValue && year.HasValue ? new DateTime(year.Value, month.Value, 1) : (DateTime?)null;
        var to = from?.AddMonths(1).AddDays(-1);
        var plans = await _repo.GetByClassAndSubjectAsync(classId, subjectId ?? Guid.Empty, from, to);
        return plans.Select(p => Map(p)).ToList();
    }

    public async Task<LessonPlanResponse> CreateAsync(CreateLessonPlanRequest request, Guid orgId, Guid teacherId)
    {
        var plan = new LessonPlan
        {
            ClassId = request.ClassId, SubjectId = request.SubjectId, TeacherId = teacherId,
            Date = request.Date, Topic = request.Topic, Objectives = request.Objectives,
            Content = request.Content, Methodology = request.Methodology,
            Resources = request.Resources, Homework = request.Homework,
            Notes = request.Notes, OrganizationId = orgId
        };
        plan = await _repo.CreateAsync(plan);
        return Map(plan);
    }

    public async Task<LessonPlanResponse> UpdateAsync(Guid id, UpdateLessonPlanRequest request)
    {
        var plan = await _repo.GetByIdAsync(id);
        if (plan == null) throw new KeyNotFoundException("Plano de aula nao encontrado");
        if (request.Topic != null) plan.Topic = request.Topic;
        if (request.Objectives != null) plan.Objectives = request.Objectives;
        if (request.Content != null) plan.Content = request.Content;
        if (request.Methodology != null) plan.Methodology = request.Methodology;
        if (request.Resources != null) plan.Resources = request.Resources;
        if (request.Homework != null) plan.Homework = request.Homework;
        if (request.Notes != null) plan.Notes = request.Notes;
        await _repo.UpdateAsync(plan);
        return Map(plan);
    }

    public async Task DeleteAsync(Guid id) => await _repo.DeleteAsync(id);

    private static LessonPlanResponse Map(LessonPlan p) => new(
        p.Id, p.ClassId, p.Class?.Name ?? "", p.SubjectId, p.Subject?.Name ?? "",
        p.TeacherId, p.Teacher?.User?.Name ?? "", p.Date,
        p.Topic, p.Objectives, p.Content, p.Methodology, p.Resources, p.Homework, p.Notes, p.CreatedAt
    );
}
