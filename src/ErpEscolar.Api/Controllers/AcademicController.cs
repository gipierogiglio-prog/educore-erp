using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErpEscolar.Api.Controllers;

using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AcademicController : ControllerBase
{
    private readonly IAcademicService _service;
    public AcademicController(IAcademicService service) => _service = service;

    private Guid? GetOrgId()
    {
        var val = User.FindFirstValue("organizationId");
        if (Guid.TryParse(val, out var id)) return id;
        return null;
    }

    // ===================== CLASSES =====================

    [HttpGet("classes")]
    public async Task<IActionResult> GetClasses()
    {
        var orgId = GetOrgId();
        if (orgId == null) return BadRequest(new { message = "Usuário sem organização" });
        return Ok(await _service.GetClassesAsync(orgId.Value));
    }

    [HttpPost("classes")]
    public async Task<IActionResult> CreateClass([FromBody] CreateClassRequest request)
    {
        var orgId = GetOrgId();
        if (orgId == null) return BadRequest(new { message = "Usuário sem organização" });
        var result = await _service.CreateClassAsync(request.Name, request.Shift, request.Room, orgId.Value);
        return CreatedAtAction(nameof(GetClasses), null, result);
    }

    [HttpPut("classes/{id}")]
    public async Task<IActionResult> UpdateClass(Guid id, [FromBody] UpdateClassRequest request)
    {
        var orgId = GetOrgId();
        if (orgId == null) return BadRequest(new { message = "Usuário sem organização" });
        try
        {
            var result = await _service.UpdateClassAsync(id, request.Name, request.Shift, request.Room, request.CourseId, orgId.Value);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpDelete("classes/{id}")]
    public async Task<IActionResult> DeleteClass(Guid id)
    {
        try
        {
            await _service.DeleteClassAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // ===================== SUBJECTS =====================

    [HttpGet("subjects")]
    public async Task<IActionResult> GetSubjects()
    {
        var orgId = GetOrgId();
        if (orgId == null) return BadRequest(new { message = "Usuário sem organização" });
        return Ok(await _service.GetSubjectsAsync(orgId.Value));
    }

    [HttpPost("subjects")]
    public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectRequest request)
    {
        var orgId = GetOrgId();
        if (orgId == null) return BadRequest(new { message = "Usuário sem organização" });
        var result = await _service.CreateSubjectAsync(request.Name, request.Code, request.Workload, orgId.Value);
        return CreatedAtAction(nameof(GetSubjects), null, result);
    }

    [HttpPut("subjects/{id}")]
    public async Task<IActionResult> UpdateSubject(Guid id, [FromBody] UpdateSubjectRequest request)
    {
        var orgId = GetOrgId();
        if (orgId == null) return BadRequest(new { message = "Usuário sem organização" });
        try
        {
            var result = await _service.UpdateSubjectAsync(id, request.Name, request.Code, request.Workload, request.CourseId, orgId.Value);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpDelete("subjects/{id}")]
    public async Task<IActionResult> DeleteSubject(Guid id)
    {
        try
        {
            await _service.DeleteSubjectAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // ===================== TEACHER ASSIGNMENT =====================

    [HttpPost("assign-teacher")]
    public async Task<IActionResult> AssignTeacher([FromBody] AssignTeacherRequest request)
    {
        var orgId = GetOrgId();
        if (orgId == null) return BadRequest(new { message = "Usuário sem organização" });
        try
        {
            await _service.AssignTeacherAsync(request.TeacherId, request.SubjectId, request.ClassId, orgId.Value);
            return Ok(new { message = "Professor atribuído com sucesso" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ===================== GRADES =====================

    [HttpGet("grades/{classId}")]
    public async Task<IActionResult> GetGrades(Guid classId, [FromQuery] int bimester, [FromQuery] int? year)
    {
        var result = await _service.GetGradesByClassAsync(classId, bimester, year ?? DateTime.UtcNow.Year);
        return Ok(result);
    }

    [HttpPost("grades/batch")]
    public async Task<IActionResult> SubmitGrades([FromBody] GradeBatchRequest request)
    {
        var orgId = GetOrgId();
        if (orgId == null) return BadRequest(new { message = "Usuário sem organização" });
        await _service.SubmitGradesAsync(request, orgId.Value);
        return Ok(new { message = "Notas lançadas com sucesso" });
    }

    // ===================== ATTENDANCE =====================

    [HttpPost("attendance/batch")]
    public async Task<IActionResult> SubmitAttendance([FromBody] AttendanceBatchRequest request)
    {
        var orgId = GetOrgId();
        if (orgId == null) return BadRequest(new { message = "Usuário sem organização" });
        await _service.SubmitAttendanceAsync(request, orgId.Value);
        return Ok(new { message = "Frequência registrada com sucesso" });
    }

    // ===================== REPORT CARD =====================

    [HttpGet("report-card/{studentId}")]
    public async Task<IActionResult> GetReportCard(Guid studentId, [FromQuery] int? year)
    {
        try
        {
            var result = await _service.GetStudentReportCardAsync(studentId, year ?? DateTime.UtcNow.Year);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // ===================== ATTENDANCE SUMMARY =====================

    [HttpGet("attendance/summary")]
    public async Task<IActionResult> GetAttendanceSummary(
        [FromQuery] Guid classId, [FromQuery] Guid subjectId,
        [FromQuery] int? year, [FromQuery] int? month)
    {
        var result = await _service.GetAttendanceSummaryAsync(classId, subjectId, year ?? DateTime.UtcNow.Year, month);
        return Ok(result);
    }
}

// === Request Records ===
public record CreateClassRequest(string Name, string Shift, string? Room);
public record UpdateClassRequest(string Name, string Shift, string? Room, Guid? CourseId);
public record CreateSubjectRequest(string Name, string Code, int Workload);
public record UpdateSubjectRequest(string Name, string Code, int? Workload, Guid? CourseId);
public record AssignTeacherRequest(Guid TeacherId, Guid SubjectId, Guid ClassId);
