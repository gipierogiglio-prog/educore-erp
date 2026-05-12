using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErpEscolar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AcademicController : ControllerBase
{
    private readonly IAcademicService _service;
    public AcademicController(IAcademicService service) => _service = service;

    // === Classes ===

    [HttpGet("classes")]
    public async Task<IActionResult> GetClasses() => Ok(await _service.GetClassesAsync());

    [HttpPost("classes")]
    public async Task<IActionResult> CreateClass([FromBody] CreateClassRequest request)
    {
        var result = await _service.CreateClassAsync(request.Name, request.Shift, request.Room);
        return CreatedAtAction(nameof(GetClasses), null, result);
    }

    // === Subjects ===

    [HttpGet("subjects")]
    public async Task<IActionResult> GetSubjects() => Ok(await _service.GetSubjectsAsync());

    [HttpPost("subjects")]
    public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectRequest request)
    {
        var result = await _service.CreateSubjectAsync(request.Name, request.Code, request.Workload);
        return CreatedAtAction(nameof(GetSubjects), null, result);
    }

    // === Teacher Assignment ===

    [HttpPost("assign-teacher")]
    public async Task<IActionResult> AssignTeacher([FromBody] AssignTeacherRequest request)
    {
        try
        {
            await _service.AssignTeacherAsync(request.TeacherId, request.SubjectId, request.ClassId);
            return Ok(new { message = "Professor atribuído com sucesso" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // === Grades ===

    [HttpGet("grades/{classId}")]
    public async Task<IActionResult> GetGrades(Guid classId, [FromQuery] int bimester, [FromQuery] int? year)
    {
        var result = await _service.GetGradesByClassAsync(classId, bimester, year ?? DateTime.UtcNow.Year);
        return Ok(result);
    }

    [HttpPost("grades/batch")]
    public async Task<IActionResult> SubmitGrades([FromBody] GradeBatchRequest request)
    {
        await _service.SubmitGradesAsync(request);
        return Ok(new { message = "Notas lançadas com sucesso" });
    }

    // === Attendance ===

    [HttpPost("attendance/batch")]
    public async Task<IActionResult> SubmitAttendance([FromBody] AttendanceBatchRequest request)
    {
        await _service.SubmitAttendanceAsync(request);
        return Ok(new { message = "Frequência registrada com sucesso" });
    }
}

public record CreateClassRequest(string Name, string Shift, string? Room);
public record CreateSubjectRequest(string Name, string Code, int Workload);
public record AssignTeacherRequest(Guid TeacherId, Guid SubjectId, Guid ClassId);
