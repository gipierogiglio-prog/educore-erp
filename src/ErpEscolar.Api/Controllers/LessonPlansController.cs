using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ErpEscolar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LessonPlansController : ControllerBase
{
    private readonly ILessonPlanService _service;
    public LessonPlansController(ILessonPlanService service) => _service = service;

    private Guid? GetTeacherId()
    {
        var val = User.FindFirstValue("teacherId");
        if (Guid.TryParse(val, out var id)) return id;
        return null;
    }

    [HttpGet("class/{classId}")]
    public async Task<IActionResult> GetByClass(Guid classId, [FromQuery] Guid? subjectId, [FromQuery] int? month, [FromQuery] int? year)
    {
        return Ok(await _service.GetByClassAsync(classId, subjectId, month, year));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLessonPlanRequest request)
    {
        var orgVal = User.FindFirstValue("organizationId");
        if (!Guid.TryParse(orgVal, out var orgId))
            return BadRequest(new { message = "Usuario sem organizacao" });

        var teacherId = GetTeacherId() ?? Guid.Empty;
        var result = await _service.CreateAsync(request, orgId, teacherId);
        return CreatedAtAction(nameof(GetByClass), new { classId = request.ClassId }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLessonPlanRequest request)
    {
        try
        {
            var result = await _service.UpdateAsync(id, request);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
