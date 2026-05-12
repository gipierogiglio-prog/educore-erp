using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;

namespace ErpEscolar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SchoolYearsController : ControllerBase
{
    private readonly ISchoolYearService _service;

    public SchoolYearsController(ISchoolYearService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var years = await _service.GetAllAsync();
        return Ok(years);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var year = await _service.GetByIdAsync(id);
        if (year == null) return NotFound();
        return Ok(year);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSchoolYearRequest request)
    {
        try
        {
            var year = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = year.Id }, year);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateSchoolYearStatusRequest request)
    {
        try
        {
            await _service.UpdateStatusAsync(id, request.Status);
            return Ok(new { message = "Status atualizado" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Ano letivo não encontrado" });
        }
    }
}

public record UpdateSchoolYearStatusRequest(string Status);
