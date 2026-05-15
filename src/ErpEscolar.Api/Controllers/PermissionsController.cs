using ErpEscolar.Core.Entities;
using ErpEscolar.Infra.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ErpEscolar.Infra.Data;
using System.Security.Claims;

namespace ErpEscolar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPermissionService _permService;

    public PermissionsController(AppDbContext db, IPermissionService permService)
    {
        _db = db;
        _permService = permService;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private Guid? GetOrgId()
    {
        var val = User.FindFirstValue("organizationId");
        if (Guid.TryParse(val, out var id)) return id;
        return null;
    }

    // === Permissões Globais (lista fixa) ===

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var perms = await _db.Permissions.OrderBy(p => p.Resource).ThenBy(p => p.Action)
            .Select(p => new { p.Id, p.Resource, p.Action, p.Name })
            .ToListAsync();
        return Ok(perms);
    }

    // === Grupos de Permissão ===

    [HttpGet("groups")]
    public async Task<IActionResult> GetGroups()
    {
        var orgId = GetOrgId();
        if (orgId == null) return BadRequest(new { message = "Sem organização" });

        var groups = await _db.PermissionGroups
            .Include(pg => pg.GroupPermissions).ThenInclude(gp => gp.Permission)
            .Where(pg => pg.OrganizationId == orgId)
            .Select(pg => new
            {
                pg.Id,
                pg.Name,
                pg.Description,
                Permissions = pg.GroupPermissions.Select(gp => new { gp.PermissionId, gp.Permission.Resource, gp.Permission.Action, gp.Permission.Name })
            })
            .ToListAsync();
        return Ok(groups);
    }

    [HttpPost("groups")]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
    {
        var orgId = GetOrgId();
        if (orgId == null) return BadRequest(new { message = "Sem organização" });
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { message = "Nome é obrigatório" });

        var group = new PermissionGroup
        {
            Name = request.Name,
            Description = request.Description,
            OrganizationId = orgId.Value,
        };

        if (request.PermissionIds?.Any() == true)
        {
            foreach (var permId in request.PermissionIds)
            {
                group.GroupPermissions.Add(new GroupPermission { PermissionId = permId });
            }
        }

        _db.PermissionGroups.Add(group);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetGroups), new { id = group.Id }, new { group.Id, group.Name, group.Description });
    }

    [HttpPut("groups/{id}")]
    public async Task<IActionResult> UpdateGroup(Guid id, [FromBody] CreateGroupRequest request)
    {
        var orgId = GetOrgId();
        var group = await _db.PermissionGroups.Include(pg => pg.GroupPermissions)
            .FirstOrDefaultAsync(pg => pg.Id == id && pg.OrganizationId == orgId);
        if (group == null) return NotFound();

        group.Name = request.Name ?? group.Name;
        group.Description = request.Description;

        // Atualizar permissões do grupo
        if (request.PermissionIds != null)
        {
            _db.GroupPermissions.RemoveRange(group.GroupPermissions);
            foreach (var permId in request.PermissionIds)
            {
                group.GroupPermissions.Add(new GroupPermission { PermissionId = permId });
            }
        }

        await _db.SaveChangesAsync();
        return Ok(new { message = "Grupo atualizado" });
    }

    [HttpDelete("groups/{id}")]
    public async Task<IActionResult> DeleteGroup(Guid id)
    {
        var orgId = GetOrgId();
        var group = await _db.PermissionGroups
            .FirstOrDefaultAsync(pg => pg.Id == id && pg.OrganizationId == orgId);
        if (group == null) return NotFound();

        _db.PermissionGroups.Remove(group);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Grupo removido" });
    }

    // === Usuários × Grupos ===

    [HttpPost("users/{userId}/groups")]
    public async Task<IActionResult> AssignGroup(Guid userId, [FromBody] AssignGroupRequest request)
    {
        var orgId = GetOrgId();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && u.OrganizationId == orgId);
        if (user == null) return NotFound(new { message = "Usuário não encontrado" });

        var exists = await _db.UserGroups.AnyAsync(ug => ug.UserId == userId && ug.GroupId == request.GroupId);
        if (exists) return BadRequest(new { message = "Usuário já pertence a este grupo" });

        _db.UserGroups.Add(new UserGroup { UserId = userId, GroupId = request.GroupId });
        await _db.SaveChangesAsync();
        return Ok(new { message = "Grupo atribuído" });
    }

    [HttpDelete("users/{userId}/groups/{groupId}")]
    public async Task<IActionResult> RemoveGroup(Guid userId, Guid groupId)
    {
        var ug = await _db.UserGroups.FirstOrDefaultAsync(x => x.UserId == userId && x.GroupId == groupId);
        if (ug == null) return NotFound();
        _db.UserGroups.Remove(ug);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Grupo removido" });
    }

    // === Permissões Diretas do Usuário ===

    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUserPermissions(Guid userId)
    {
        var perms = await _permService.GetUserPermissionsAsync(userId);
        return Ok(perms);
    }

    [HttpPost("users/{userId}/direct")]
    public async Task<IActionResult> SetDirectPermission(Guid userId, [FromBody] SetPermissionRequest request)
    {
        var orgId = GetOrgId();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && u.OrganizationId == orgId);
        if (user == null) return NotFound(new { message = "Usuário não encontrado" });

        await _db.UserPermissions
            .Where(up => up.UserId == userId && up.PermissionId == request.PermissionId)
            .ExecuteDeleteAsync();

        _db.UserPermissions.Add(new UserPermission
        {
            UserId = userId,
            PermissionId = request.PermissionId,
            Granted = request.Granted,
        });
        await _db.SaveChangesAsync();
        return Ok(new { message = request.Granted ? "Permissão concedida" : "Permissão negada" });
    }
}

public record CreateGroupRequest(string Name, string? Description, List<Guid>? PermissionIds);
public record AssignGroupRequest(Guid GroupId);
public record SetPermissionRequest(Guid PermissionId, bool Granted);
