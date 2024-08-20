using Microsoft.AspNetCore.Authorization;

namespace PermissionTest.API.Auth;

public class HasPermissionRequirement : IAuthorizationRequirement
{
    public Permissions Permission { get; }

    public HasPermissionRequirement(Permissions permission)
    {
        Permission = permission;
    }

}