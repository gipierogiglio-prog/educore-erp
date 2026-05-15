using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ErpEscolar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;

    public StudentsController(IStudentService service) => _service = service;

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
        var students = await _service.GetAllAsync(orgId.Value);
        return Ok(students);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var student = await _service.GetByIdAsync(id);
        if (student == null) return NotFound();
        return Ok(student);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentRequest request)
    {
        try
        {
            var orgId = GetOrgId();
            if (orgId == null) return BadRequest(new { message = "Usuário sem organização" });
            var student = await _service.CreateAsync(request, orgId.Value);
            return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/toggle-status")]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        var active = await _service.ToggleStatusAsync(id);
        if (active == null) return NotFound();
        return Ok(new { active });
    }

    [HttpPatch("{id}/class")]
    public async Task<IActionResult> UpdateClass(Guid id, [FromBody] UpdateClassRequest request)
    {
        try
        {
            await _service.UpdateClassAsync(id, request.ClassId);
            return Ok(new { message = "Turma atualizada" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}

