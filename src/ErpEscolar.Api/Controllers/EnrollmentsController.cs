using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;
using System.Security.Claims;

namespace ErpEscolar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _service;

    public EnrollmentsController(IEnrollmentService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? year)
    {
        var enrollments = await _service.GetAllAsync(year);
        return Ok(enrollments);
    }

    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetByStudent(Guid studentId)
    {
        var enrollments = await _service.GetByStudentAsync(studentId);
        return Ok(enrollments);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEnrollmentRequest request)
    {
        try
        {
            var enrollment = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetAll), new { year = enrollment.SchoolYear }, enrollment);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateEnrollmentStatusRequest request)
    {
        try
        {
            await _service.UpdateStatusAsync(id, request.Status);
            return Ok(new { message = "Status atualizado" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
    {
        try
        {
            var orgId = GetOrgId();
            if (orgId == null) return BadRequest(new { message = "Usuario sem organizacao" });
            var result = await _service.TransferAsync(request, orgId.Value);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    private Guid? GetOrgId()
    {
        var val = User.FindFirstValue("organizationId");
        if (Guid.TryParse(val, out var id)) return id;
        return null;
    }
}

public record UpdateEnrollmentStatusRequest(string Status);
