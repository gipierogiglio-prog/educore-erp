using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ErpEscolar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StaffController : ControllerBase
{
    private readonly IStaffService _service;
    public StaffController(IStaffService service) => _service = service;

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
        if (orgId == null) return BadRequest(new { message = "Usuario sem organizacao" });
        return Ok(await _service.GetAllAsync(orgId.Value));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStaffRequest request)
    {
        var orgId = GetOrgId();
        if (orgId == null) return BadRequest(new { message = "Usuario sem organizacao" });
        try
        {
            var result = await _service.CreateAsync(request, orgId.Value);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStaffRequest request)
    {
        var orgId = GetOrgId();
        if (orgId == null) return BadRequest(new { message = "Usuario sem organizacao" });
        try
        {
            var result = await _service.UpdateAsync(id, request, orgId.Value);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/toggle-status")]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        try
        {
            await _service.ToggleStatusAsync(id);
            return Ok(new { message = "Status alterado" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
