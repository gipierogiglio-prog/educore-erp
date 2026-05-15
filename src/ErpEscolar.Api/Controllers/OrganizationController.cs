using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
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

    [HttpPost("sync-status")]
    [AllowAnonymous]
    public async Task<IActionResult> SyncStatus([FromBody] SyncStatusRequest request)
    {
        if (string.IsNullOrEmpty(request.OrganizationId))
            return BadRequest(new { message = "organizationId obrigatório" });

        if (!Guid.TryParse(request.OrganizationId, out var orgId))
            return BadRequest(new { message = "organizationId inválido" });

        var org = await _db.Organizations.FindAsync(orgId);
        if (org == null)
            return NotFound(new { message = "Organização não encontrada" });

        org.Status = request.Status;

        // Ao desativar a org, desativa todos os usuários
        // Ao reativar a org, NÃO reativa usuários (evita ativar quem foi desativado individualmente)
        if (request.Status == "inactive")
        {
            var users = await _db.Users.Where(u => u.OrganizationId == orgId && u.Active).ToListAsync();
            foreach (var u in users)
            {
                u.Active = false;
            }
            await _db.SaveChangesAsync();
            return Ok(new { message = $"Organização desativada, {users.Count} usuários desativados" });
        }

        await _db.SaveChangesAsync();
        return Ok(new { message = $"Organização reativada. Usuários inativos precisam ser reativados manualmente." });
    }

    public record SyncStatusRequest(string OrganizationId, string Status);
}
