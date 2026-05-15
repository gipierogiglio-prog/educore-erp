namespace ErpEscolar.Core.Entities;

public class Permission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Resource { get; set; } = "";  // ex: "students", "grades", "reports"
    public string Action { get; set; } = "";     // ex: "view", "create", "edit", "delete"
    public string Name { get; set; } = "";       // ex: "Visualizar Alunos"
    public string? Description { get; set; }
}

public class PermissionGroup
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public Guid OrganizationId { get; set; }

    public Organization Organization { get; set; } = null!;
    public ICollection<GroupPermission> GroupPermissions { get; set; } = new List<GroupPermission>();
    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
}

public class GroupPermission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid GroupId { get; set; }
    public Guid PermissionId { get; set; }

    public PermissionGroup Group { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}

public class UserPermission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid PermissionId { get; set; }
    public bool Granted { get; set; } = true; // true = permitir, false = negar (sobrescreve grupo)

    public User User { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}

public class UserGroup
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }

    public User User { get; set; } = null!;
    public PermissionGroup Group { get; set; } = null!;
}
