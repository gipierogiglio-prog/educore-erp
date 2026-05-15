using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ErpEscolar.Infra.Data;
using ErpEscolar.Core.Entities;

namespace ErpEscolar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public UsersController(AppDbContext db) => _db = db;

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
        if (orgId == null) return BadRequest(new { message = "Sem organização" });

        var users = await _db.Users
            .Where(u => u.OrganizationId == orgId)
            .OrderBy(u => u.Name)
            .Select(u => new
            {
                u.Id, u.Name, u.Email, u.Role, u.Active,
                Groups = _db.UserGroups.Where(ug => ug.UserId == u.Id).Select(ug => ug.Group.Name).ToList()
            })
            .ToListAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var orgId = GetOrgId();
        var user = await _db.Users
            .Where(u => u.Id == id && u.OrganizationId == orgId)
            .Select(u => new
            {
                u.Id, u.Name, u.Email, u.Role, u.Active,
                u.Phone,
                u.OrganizationId
            })
            .FirstOrDefaultAsync();
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var orgId = GetOrgId();
        if (orgId == null) return BadRequest(new { message = "Sem organização" });

        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest(new { message = "Email já cadastrado" });

        if (!new[] { "teacher", "coordinator", "student", "guardian" }.Contains(request.Role))
            return BadRequest(new { message = "Role inválida. Use: teacher, coordinator, student, guardian" });

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            Phone = request.Phone,
            Active = true,
            OrganizationId = orgId,
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, new { user.Id, user.Name, user.Email, user.Role });
    }

    [HttpPatch("{id}/toggle-status")]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        var orgId = GetOrgId();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id && u.OrganizationId == orgId);
        if (user == null) return NotFound();

        user.Active = !user.Active;
        user.AutoDeactivated = false; // reativacao manual limpa flag
        await _db.SaveChangesAsync();
        return Ok(new { active = user.Active });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var orgId = GetOrgId();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id && u.OrganizationId == orgId);
        if (user == null) return NotFound();

        if (!string.IsNullOrEmpty(request.Name)) user.Name = request.Name;
        if (!string.IsNullOrEmpty(request.Phone)) user.Phone = request.Phone;
        if (!string.IsNullOrEmpty(request.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        await _db.SaveChangesAsync();
        return Ok(new { message = "Usuário atualizado" });
    }
}

public record CreateUserRequest(string Name, string Email, string Password, string Role, string? Phone);
public record UpdateUserRequest(string? Name, string? Phone, string? Password);
