using ErpEscolar.Infra.Data;

using ErpEscolar.Core.Entities;
using ErpEscolar.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ErpEscolar.Infra.Services;

public interface IPermissionService
{
    Task<bool> UserHasPermissionAsync(Guid userId, string resource, string action);
    Task<List<string>> GetUserPermissionsAsync(Guid userId);
    Task SeedDefaultPermissionsAsync();
}

public class PermissionService : IPermissionService
{
    private readonly AppDbContext _db;

    public PermissionService(AppDbContext db) => _db = db;

    public async Task<bool> UserHasPermissionAsync(Guid userId, string resource, string action)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return false;
        if (user.Role == "super_admin" || user.Role == "org_admin") return true;

        // Buscar permissão pelo resource+action
        var permission = await _db.Permissions
            .FirstOrDefaultAsync(p => p.Resource == resource && p.Action == action);
        if (permission == null) return false;

        // 1. Verificar permissões diretas do usuário
        var directPerm = await _db.UserPermissions
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permission.Id);
        if (directPerm != null) return directPerm.Granted;

        // 2. Verificar grupos do usuário
        var groupPerms = await _db.UserGroups
            .Where(ug => ug.UserId == userId)
            .SelectMany(ug => ug.Group.GroupPermissions)
            .AnyAsync(gp => gp.PermissionId == permission.Id);

        return groupPerms;
    }

    public async Task<List<string>> GetUserPermissionsAsync(Guid userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null || user.Role == "super_admin")
            return new List<string> { "*" };

        var permissions = new List<string>();

        // Permissões diretas concedidas
        var directGranted = await _db.UserPermissions
            .Where(up => up.UserId == userId && up.Granted)
            .Select(up => up.Permission.Resource + "." + up.Permission.Action)
            .ToListAsync();
        permissions.AddRange(directGranted);

        // Permissões de grupos
        var groupPerms = await _db.UserGroups
            .Where(ug => ug.UserId == userId)
            .SelectMany(ug => ug.Group.GroupPermissions)
            .Select(gp => gp.Permission.Resource + "." + gp.Permission.Action)
            .ToListAsync();
        permissions.AddRange(groupPerms);

        // Excluir permissões negadas diretamente
        var denied = await _db.UserPermissions
            .Where(up => up.UserId == userId && !up.Granted)
            .Select(up => up.Permission.Resource + "." + up.Permission.Action)
            .ToListAsync();

        return permissions.Except(denied).Distinct().ToList();
    }

    public async Task SeedDefaultPermissionsAsync()
    {
        if (await _db.Permissions.AnyAsync()) return;

        var defaultPerms = new List<Permission>
        {
            // Alunos
            new() { Resource = "students", Action = "view", Name = "Visualizar Alunos" },
            new() { Resource = "students", Action = "create", Name = "Cadastrar Alunos" },
            new() { Resource = "students", Action = "edit", Name = "Editar Alunos" },
            new() { Resource = "students", Action = "delete", Name = "Excluir Alunos" },
            // Professores
            new() { Resource = "teachers", Action = "view", Name = "Visualizar Professores" },
            new() { Resource = "teachers", Action = "create", Name = "Cadastrar Professores" },
            new() { Resource = "teachers", Action = "edit", Name = "Editar Professores" },
            // Turmas
            new() { Resource = "classes", Action = "view", Name = "Visualizar Turmas" },
            new() { Resource = "classes", Action = "create", Name = "Cadastrar Turmas" },
            new() { Resource = "classes", Action = "edit", Name = "Editar Turmas" },
            // Disciplinas
            new() { Resource = "subjects", Action = "view", Name = "Visualizar Disciplinas" },
            new() { Resource = "subjects", Action = "create", Name = "Cadastrar Disciplinas" },
            new() { Resource = "subjects", Action = "edit", Name = "Editar Disciplinas" },
            // Matrículas
            new() { Resource = "enrollments", Action = "view", Name = "Visualizar Matrículas" },
            new() { Resource = "enrollments", Action = "create", Name = "Realizar Matrículas" },
            new() { Resource = "enrollments", Action = "edit", Name = "Editar Matrículas" },
            // Notas
            new() { Resource = "grades", Action = "view", Name = "Visualizar Notas" },
            new() { Resource = "grades", Action = "create", Name = "Lançar Notas" },
            new() { Resource = "grades", Action = "edit", Name = "Editar Notas" },
            // Frequência
            new() { Resource = "attendance", Action = "view", Name = "Visualizar Frequência" },
            new() { Resource = "attendance", Action = "create", Name = "Registrar Frequência" },
            new() { Resource = "attendance", Action = "edit", Name = "Editar Frequência" },
            // Relatórios
            new() { Resource = "reports", Action = "view", Name = "Visualizar Relatórios" },
            new() { Resource = "reports", Action = "export", Name = "Exportar Relatórios" },
            // Financeiro
            new() { Resource = "financial", Action = "view", Name = "Visualizar Financeiro" },
            new() { Resource = "financial", Action = "manage", Name = "Gerenciar Financeiro" },
            // Usuários
            new() { Resource = "users", Action = "view", Name = "Visualizar Usuários" },
            new() { Resource = "users", Action = "create", Name = "Cadastrar Usuários" },
            new() { Resource = "users", Action = "edit", Name = "Editar Usuários" },
            // Permissões
            new() { Resource = "permissions", Action = "manage", Name = "Gerenciar Permissões" },
            // Anos Letivos
            new() { Resource = "schoolyears", Action = "view", Name = "Visualizar Anos Letivos" },
            new() { Resource = "schoolyears", Action = "manage", Name = "Gerenciar Anos Letivos" },
            // Organização
            new() { Resource = "organization", Action = "edit", Name = "Editar Dados da Escola" },
        };
        _db.Permissions.AddRange(defaultPerms);
        await _db.SaveChangesAsync();
    }
}
