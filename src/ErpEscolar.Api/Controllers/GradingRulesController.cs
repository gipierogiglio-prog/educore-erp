using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ErpEscolar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GradingRulesController : ControllerBase
{
    private readonly IGradingRuleService _service;
    public GradingRulesController(IGradingRuleService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var orgVal = User.FindFirstValue("organizationId");
        if (!Guid.TryParse(orgVal, out var orgId))
            return BadRequest(new { message = "Usuario sem organizacao" });

        var rule = await _service.GetAsync(orgId);
        if (rule == null) return NotFound(new { message = "Nenhuma regra configurada" });
        return Ok(rule);
    }

    [HttpPut]
    public async Task<IActionResult> Save([FromBody] UpsertGradingRuleRequest request)
    {
        var orgVal = User.FindFirstValue("organizationId");
        if (!Guid.TryParse(orgVal, out var orgId))
            return BadRequest(new { message = "Usuario sem organizacao" });

        var result = await _service.SaveAsync(request, orgId);
        return Ok(result);
    }
}
