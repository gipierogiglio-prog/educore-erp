using ErpEscolar.Infra.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace ErpEscolar.Api.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string _resource;
    private readonly string _action;

    public RequirePermissionAttribute(string resource, string action)
    {
        _resource = resource;
        _action = action;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (user.Identity?.IsAuthenticated != true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var role = user.FindFirstValue(ClaimTypes.Role);
        if (role == "super_admin" || role == "org_admin")
            return; // Admins têm acesso total

        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            context.Result = new ForbidResult();
            return;
        }

        var permService = context.HttpContext.RequestServices.GetRequiredService<IPermissionService>();
        var hasPerm = await permService.UserHasPermissionAsync(Guid.Parse(userId), _resource, _action);

        if (!hasPerm)
        {
            context.Result = new ObjectResult(new { message = $"Acesso negado: você não tem permissão para '{_resource}.{_action}'" })
            {
                StatusCode = 403
            };
        }
    }
}
