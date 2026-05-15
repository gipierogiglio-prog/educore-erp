using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ErpEscolar.Infra.Data;

namespace ErpEscolar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db) => _db = db;

    private Guid? GetOrgId()
    {
        var val = User.FindFirstValue("organizationId");
        if (Guid.TryParse(val, out var id)) return id;
        return null;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var orgId = GetOrgId();
        if (orgId == null) return BadRequest(new { message = "Sem organização" });

        var totalStudents = await _db.Students.CountAsync(s => s.OrganizationId == orgId && s.Active);
        var totalTeachers = await _db.Teachers.CountAsync(t => t.OrganizationId == orgId && t.Active);
        var totalClasses = await _db.Classes.CountAsync(c => c.OrganizationId == orgId && c.Active);
        var totalUsers = await _db.Users.CountAsync(u => u.OrganizationId == orgId && u.Active);

        // Ultimos alunos cadastrados
        var recentStudents = await _db.Students
            .Where(s => s.OrganizationId == orgId && s.Active)
            .Include(s => s.User)
            .OrderByDescending(s => s.EnrollmentDate)
            .Take(5)
            .Select(s => new { s.Id, Name = s.User.Name, s.Enrollment, s.EnrollmentDate })
            .ToListAsync();

        // Alunos por turma
        var studentsByClass = await _db.Classes
            .Where(c => c.OrganizationId == orgId && c.Active)
            .Select(c => new { c.Name, Count = c.Students.Count(s => s.Active) })
            .ToListAsync();

        return Ok(new
        {
            totalStudents,
            totalTeachers,
            totalClasses,
            totalUsers,
            recentStudents,
            studentsByClass
        });
    }
}
