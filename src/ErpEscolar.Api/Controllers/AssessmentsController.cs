using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ErpEscolar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssessmentsController : ControllerBase
{
    private readonly IAssessmentService _service;
    public AssessmentsController(IAssessmentService service) => _service = service;

    [HttpGet("class/{classId}")]
    public async Task<IActionResult> GetByClass(Guid classId, [FromQuery] int? bimester, [FromQuery] int? year)
    {
        return Ok(await _service.GetByClassAsync(classId, bimester, year));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAssessmentRequest request)
    {
        var orgVal = User.FindFirstValue("organizationId");
        if (!Guid.TryParse(orgVal, out var orgId))
            return BadRequest(new { message = "Usuario sem organizacao" });

        var result = await _service.CreateAsync(request, orgId);
        return CreatedAtAction(nameof(GetByClass), new { classId = request.ClassId }, result);
    }

    [HttpPost("{assessmentId}/grades")]
    public async Task<IActionResult> SubmitGrades(Guid assessmentId, [FromBody] List<SubmitGradeItem> grades)
    {
        try
        {
            await _service.SubmitGradesAsync(assessmentId, grades);
            return Ok(new { message = "Notas lancadas com sucesso" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{assessmentId}/grades")]
    public async Task<IActionResult> GetGrades(Guid assessmentId)
    {
        return Ok(await _service.GetGradesAsync(assessmentId));
    }
}
