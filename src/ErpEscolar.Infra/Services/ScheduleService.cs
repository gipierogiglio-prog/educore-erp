using ErpEscolar.Core.Entities;
using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;

namespace ErpEscolar.Infra.Services;

public class ScheduleService : IScheduleService
{
    private readonly IScheduleEntryRepository _repo;

    public ScheduleService(IScheduleEntryRepository repo) => _repo = repo;

    public async Task<List<ScheduleResponse>> GetByClassAsync(Guid classId)
    {
        var entries = await _repo.GetByClassAsync(classId);
        return entries.Select(Map).ToList();
    }

    public async Task<List<ScheduleResponse>> GetByTeacherAsync(Guid teacherId)
    {
        var entries = await _repo.GetByTeacherAsync(teacherId);
        return entries.Select(Map).ToList();
    }

    public async Task<ScheduleResponse> CreateAsync(CreateScheduleRequest request, Guid orgId)
    {
        var entry = new ScheduleEntry
        {
            ClassId = request.ClassId, SubjectId = request.SubjectId,
            TeacherId = request.TeacherId, DayOfWeek = request.DayOfWeek,
            StartTime = TimeSpan.Parse(request.StartTime),
            EndTime = TimeSpan.Parse(request.EndTime),
            Room = request.Room, OrganizationId = orgId
        };
        entry = await _repo.CreateAsync(entry);
        return Map(entry);
    }

    public async Task DeleteAsync(Guid id) => await _repo.DeleteAsync(id);

    private static ScheduleResponse Map(ScheduleEntry e) => new(
        e.Id, e.ClassId, e.Class?.Name ?? "", e.SubjectId, e.Subject?.Name ?? "",
        e.TeacherId, e.Teacher?.User?.Name ?? "", e.DayOfWeek,
        e.StartTime.ToString(@"hh\:mm"), e.EndTime.ToString(@"hh\:mm"), e.Room
    );
}
