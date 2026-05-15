using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErpEscolar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeachersController : ControllerBase
{
    private readonly ITeacherService _service;

    public TeachersController(ITeacherService service) => _service = service;

    private Guid? GetOrgId()
    {
        var val = User.FindFirstValue("organizationId");
        if (Guid.TryParse(val, out var id)) return id;
        return null;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orgId = GetOrgId();
        if (orgId == null) return BadRequest(new { message = "Usuário sem organização" });
        return Ok(await _service.GetAllAsync(orgId.Value));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var teacher = await _service.GetByIdAsync(id);
        if (teacher == null) return NotFound();
        return Ok(teacher);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTeacherRequest request)
    {
        try
        {
            var orgId = GetOrgId();
            if (orgId == null) return BadRequest(new { message = "Usuário sem organização" });
            var teacher = await _service.CreateAsync(request, orgId.Value);
            return CreatedAtAction(nameof(GetById), new { id = teacher.Id }, teacher);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
