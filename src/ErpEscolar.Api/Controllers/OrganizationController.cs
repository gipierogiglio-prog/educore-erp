using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ErpEscolar.Infra.Data;

namespace ErpEscolar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizationController : ControllerBase
{
    private readonly AppDbContext _db;

    public OrganizationController(AppDbContext db) => _db = db;

    [HttpGet("current")]
    public IActionResult GetCurrent()
    {
        var orgId = User.FindFirstValue("organizationId");
        if (string.IsNullOrEmpty(orgId) || !Guid.TryParse(orgId, out var guid))
            return Ok(new { message = "Usuário sem organização vinculada" });

        var org = _db.Organizations.Find(guid);
        if (org == null) return NotFound();
        return Ok(new { org.Id, org.Name, org.Slug, org.Status });
    }
}
