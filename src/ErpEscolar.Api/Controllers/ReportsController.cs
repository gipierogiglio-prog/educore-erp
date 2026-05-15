using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ErpEscolar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IStudentRepository _studentRepo;
    private readonly IEnrollmentRepository _enrollmentRepo;
    private readonly IClassRepository _classRepo;
    private readonly ICourseRepository _courseRepo;

    public ReportsController(
        IStudentRepository studentRepo, IEnrollmentRepository enrollmentRepo,
        IClassRepository classRepo, ICourseRepository courseRepo)
    {
        _studentRepo = studentRepo;
        _enrollmentRepo = enrollmentRepo;
        _classRepo = classRepo;
        _courseRepo = courseRepo;
    }

    private Guid? GetOrgId()
    {
        var val = User.FindFirstValue("organizationId");
        if (Guid.TryParse(val, out var id)) return id;
        return null;
    }

    [HttpGet("enrolled-students")]
    public async Task<IActionResult> GetEnrolledStudents([FromQuery] int? year, [FromQuery] Guid? classId)
    {
        var orgId = GetOrgId();
        if (orgId == null) return BadRequest(new { message = "Usuario sem organizacao" });

        var enrollments = await _enrollmentRepo.GetAllAsync(year ?? DateTime.UtcNow.Year);
        var students = await _studentRepo.GetAllAsync(orgId.Value, false);

        var query = from e in enrollments
                    join s in students on e.StudentId equals s.Id
                    where s.OrganizationId == orgId.Value
                    select new { e, s };

        if (classId.HasValue && classId != Guid.Empty)
            query = query.Where(x => x.e.ClassId == classId.Value);

        var result = query.Select(x => new EnrolledStudentsReport(
            x.s.User.Name, x.s.Enrollment, x.e.ClassName,
            x.e.ClassShift, x.s.Guardian != null ? x.s.Guardian.User.Name : null,
            x.e.Status
        )).OrderBy(r => r.StudentName).ToList();

        return Ok(result);
    }

    [HttpGet("classes-by-year")]
    public async Task<IActionResult> GetClassesByYear([FromQuery] int? year)
    {
        var orgId = GetOrgId();
        if (orgId == null) return BadRequest(new { message = "Usuario sem organizacao" });

        var classes = await _classRepo.GetAllAsync(year ?? DateTime.UtcNow.Year, orgId);
        var result = classes.Select(c => new ClassesByYearReport(
            c.Name, c.Shift, c.Students.Count(s => s.Active), 40
        )).ToList();

        return Ok(result);
    }

    [HttpGet("students-by-course")]
    public async Task<IActionResult> GetStudentsByCourse()
    {
        var orgId = GetOrgId();
        if (orgId == null) return BadRequest(new { message = "Usuario sem organizacao" });

        var courses = await _courseRepo.GetAllAsync(orgId.Value);
        var result = courses.Select(c => new StudentsByCourseReport(
            c.Name, c.Classes.Count, c.Classes.Sum(cl => cl.Students.Count(s => s.Active))
        )).ToList();

        return Ok(result);
    }
}
