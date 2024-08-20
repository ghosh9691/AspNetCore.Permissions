using Microsoft.AspNetCore.Authorization;

namespace PermissionTest.API.Auth;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(Permissions permission)
    {
        Policy = permission.ToString();
    }
}