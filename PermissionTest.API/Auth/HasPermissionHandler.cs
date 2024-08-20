using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PermissionTest.API.Models;

namespace PermissionTest.API.Auth;

public class HasPermissionHandler : AuthorizationHandler<HasPermissionRequirement>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public HasPermissionHandler(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermissionRequirement requirement)
    {
        var user = await _userManager.GetUserAsync(context.User);
        if (user == null)
        {
            return;
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var roleName in userRoles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                continue;
            }

            var roleClaims = await _roleManager.GetClaimsAsync(role);
            var hasPermission = roleClaims.Any(p => p.Type == "Permission" && 
                                                    Enum.TryParse(p.Value, out Permissions pEnum) && 
                                                    (pEnum == requirement.Permission || pEnum == Permissions.AllAccess));
            if (hasPermission)
            {
                context.Succeed(requirement);
                return;
            }
        }
    }

}