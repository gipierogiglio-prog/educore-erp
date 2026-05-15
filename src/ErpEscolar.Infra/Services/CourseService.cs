using ErpEscolar.Core.Entities;
using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;

namespace ErpEscolar.Infra.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _repo;

    public CourseService(ICourseRepository repo) => _repo = repo;

    public async Task<List<CourseResponse>> GetAllAsync(Guid orgId)
    {
        var courses = await _repo.GetAllAsync(orgId);
        return courses.Select(c => new CourseResponse(
            c.Id, c.Name, c.Description, c.DurationYears,
            c.Classes.Count, c.Subjects.Count, c.Active, c.CreatedAt
        )).ToList();
    }

    public async Task<CourseResponse?> GetByIdAsync(Guid id)
    {
        var c = await _repo.GetByIdAsync(id);
        if (c == null) return null;
        return new CourseResponse(
            c.Id, c.Name, c.Description, c.DurationYears,
            c.Classes.Count, c.Subjects.Count, c.Active, c.CreatedAt
        );
    }

    public async Task<CourseResponse> CreateAsync(CreateCourseRequest request, Guid orgId)
    {
        var course = new Course
        {
            Name = request.Name,
            Description = request.Description,
            DurationYears = request.DurationYears,
            OrganizationId = orgId
        };
        course = await _repo.CreateAsync(course);
        return new CourseResponse(course.Id, course.Name, course.Description, course.DurationYears, 0, 0, true, course.CreatedAt);
    }

    public async Task<CourseResponse> UpdateAsync(Guid id, UpdateCourseRequest request)
    {
        var course = await _repo.GetByIdAsync(id);
        if (course == null) throw new KeyNotFoundException("Curso não encontrado");

        if (request.Name != null) course.Name = request.Name;
        if (request.Description != null) course.Description = request.Description;
        if (request.DurationYears.HasValue) course.DurationYears = request.DurationYears.Value;

        await _repo.UpdateAsync(course);
        return new CourseResponse(course.Id, course.Name, course.Description, course.DurationYears,
            course.Classes.Count, course.Subjects.Count, course.Active, course.CreatedAt);
    }

    public async Task DeleteAsync(Guid id) => await _repo.DeleteAsync(id);
}
