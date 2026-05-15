using ErpEscolar.Core.Entities;
using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;

namespace ErpEscolar.Infra.Services;

public class GradingRuleService : IGradingRuleService
{
    private readonly IGradingRuleRepository _repo;

    public GradingRuleService(IGradingRuleRepository repo) => _repo = repo;

    public async Task<GradingRuleResponse?> GetAsync(Guid orgId)
    {
        var rule = await _repo.GetByOrganizationAsync(orgId);
        if (rule == null) return null;
        return Map(rule);
    }

    public async Task<GradingRuleResponse> SaveAsync(UpsertGradingRuleRequest request, Guid orgId)
    {
        var existing = await _repo.GetByOrganizationAsync(orgId);
        if (existing != null)
        {
            if (request.Name != null) existing.Name = request.Name;
            if (request.PassingGrade.HasValue) existing.PassingGrade = request.PassingGrade.Value;
            if (request.RecoveryGrade.HasValue) existing.RecoveryGrade = request.RecoveryGrade.Value;
            if (request.B1Weight.HasValue) existing.B1Weight = request.B1Weight.Value;
            if (request.B2Weight.HasValue) existing.B2Weight = request.B2Weight.Value;
            if (request.B3Weight.HasValue) existing.B3Weight = request.B3Weight.Value;
            if (request.B4Weight.HasValue) existing.B4Weight = request.B4Weight.Value;
            if (request.UseRecoveryExam.HasValue) existing.UseRecoveryExam = request.UseRecoveryExam.Value;
            if (request.RecoveryMaxScore.HasValue) existing.RecoveryMaxScore = request.RecoveryMaxScore.Value;
            await _repo.UpdateAsync(existing);
            return Map(existing);
        }

        var rule = new GradingRule
        {
            Name = request.Name ?? "Regra Padrao",
            PassingGrade = request.PassingGrade ?? 6m,
            RecoveryGrade = request.RecoveryGrade ?? 3m,
            B1Weight = request.B1Weight ?? 1m,
            B2Weight = request.B2Weight ?? 1m,
            B3Weight = request.B3Weight ?? 1m,
            B4Weight = request.B4Weight ?? 1m,
            UseRecoveryExam = request.UseRecoveryExam ?? true,
            RecoveryMaxScore = request.RecoveryMaxScore ?? 10m,
            OrganizationId = orgId
        };
        rule = await _repo.CreateAsync(rule);
        return Map(rule);
    }

    private static GradingRuleResponse Map(GradingRule r) => new(
        r.Id, r.Name, r.PassingGrade, r.RecoveryGrade,
        r.B1Weight, r.B2Weight, r.B3Weight, r.B4Weight,
        r.UseRecoveryExam, r.RecoveryMaxScore
    );
}
