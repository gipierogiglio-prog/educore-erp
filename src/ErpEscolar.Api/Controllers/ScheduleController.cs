using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ErpEscolar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _service;
    public ScheduleController(IScheduleService service) => _service = service;

    [HttpGet("class/{classId}")]
    public async Task<IActionResult> GetByClass(Guid classId) => Ok(await _service.GetByClassAsync(classId));

    [HttpGet("teacher/{teacherId}")]
    public async Task<IActionResult> GetByTeacher(Guid teacherId) => Ok(await _service.GetByTeacherAsync(teacherId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScheduleRequest request)
    {
        var orgVal = User.FindFirstValue("organizationId");
        if (!Guid.TryParse(orgVal, out var orgId))
            return BadRequest(new { message = "Usuario sem organizacao" });

        var result = await _service.CreateAsync(request, orgId);
        return CreatedAtAction(nameof(GetByClass), new { classId = request.ClassId }, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
