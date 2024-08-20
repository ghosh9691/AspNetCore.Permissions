# AspNetCore.Permissions

This repository demonstrates how to implement a custom permissions based authorization system in ASP.NET Core 8. The system uses ASP.NET Core Identity to create and managem users and roles. In addition, roles are assigned one or more permissions.

The system is implemented using a web api and a front-end using Blazor WebAssembly. It can be easily extended to support other clients such as WinForms, WPF, MAUI, etc.


## Features

- Granular permissions, for example, 'Users.Read', 'Users.Write', 'Users.Delete', 'Roles.Read', 'Roles.Write', 'Roles.Delete', etc.
- Permissions are assigned to roles.
- Roles are assigned to users.
- Users can have multiple roles.
- Users can have multiple permissions.
- Permissions can be assigned to multiple roles.
- Permissions added to Roles as Claims
- Permissions are stored in the database.
- Permissions can be cached in Memory Cache.
- Roles can be created by end-users with appropriate permissions.
- Permissions can be dynamically added to roles by end uers with appropriate permissions.
- Permissions are evaluated on the client side using custom `HasPermission` attribute.
- Authorization Policies are dynamically generated on client side using the list of Permissions

## Getting Started - Backend

Follow the normal steps to create a new ASP.NET Core Web API project. Then, add the following NuGet packages:

- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Design
- Microsoft.EntityFrameworkCore.Sqlite (or any other database provider: for example, SqlServer, MySql, etc.)
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.AspNetCore.Identity.UI (optional)

Modify the `Program.cs` file as follows:

```csharp

... (edited for brevity)
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<YourDbContext>(options => options.UseSqlite(connString));

builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<YourDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication();

builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddTransient<IEmailSender<ApplicationUser>, AuthEmailSender>();
builder.Services.AddScoped<IAuthorizationHandler, HasPermissionHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionsAuthorizationPolicyProvider>();
    
builder.Services.AddAuthorization();
```

In the above code snippet, notice the following:
- `YourDbContext` is the name of your database context class.
- `ApplicationUser` is the name of your user class. This is derived from `IdentityUser`.
- `ApplicationRole` is the name of your role class. This is derived from `IdentityRole`.
- `IEmailService` is an interface for sending emails. The implementation is up to you. You can use SendGrid, SMTP, etc.
- `AuthEmailSender` is a class that implements `IEmailSender<ApplicationUser>`. This is used to send emails to users. It is required as part of ASP.NET Core Identity.
- `HasPermissionHandler` is a class that implements `IAuthorizationHandler`. This class is used to check if a user has a specific permission.
- `PermissionsAuthorizationPolicyProvider` is a class that implements `IAuthorizationPolicyProvider`. This class is used to dynamically generate authorization policies based on the list of permissions.
- `AddAuthorization` is used to add the authorization services to the application.
- `AddIdentityApiEndpoints` is used to add the Identity services to the application.


### Permissions

The Permissions are an enumeration that is defined in the `Permissions.cs` file. It looks like:
```csharp
public enum Permissions : ushort
{
    None = 0
    UsersRead = 63001,
    UsersWrite = 63002,
    UsersDelete=63003,
    RolesRead = 63501,
    RolesWrite=63502,
    RolesDelete=63503,
    PermissionsRead = 64000,
    PermissionsWrite = 64001,,
    PermissionsDelete = 64002,
    AllAccess = ushort.MaxValue
}
```

You can add as many permissions as you need. You can also group permissions as shown above by specifying the value. For example, `PermissionsRead`, `PermissionsWrite`, and `PermissionsDelete` are grouped together. 

### HasPermissionHandler

The `HasPermissionHandler` class is used to check if a user has a specific permission. It is implemented as follows:

```csharp
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
```
The above code ensures that the user has a specific permission or has the `AllAccess` permission. The `AllAccess` permission is used to grant full access to the user, specifically, the system administrator.

### PermissionsAuthorizationPolicyProvider

The `PermissionsAuthorizationPolicyProvider` class is used to dynamically generate authorization policies based on the list of permissions. It is implemented as follows:

```csharp
public class PermissionsAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _options;

    public PermissionsAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
    {
        _options = options.Value;
    }

    public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        if (Enum.TryParse(typeof(Permissions), policyName, out var permission))
        {
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new HasPermissionRequirement((Permissions)permission))
                .Build();

            _options.AddPolicy(policyName, policy);
            return Task.FromResult(policy);
        }

        return base.GetPolicyAsync(policyName);
    }

}
```

The above code dynamically generates an authorization policy based on the permission name. If the permission name is found in the `Permissions` enumeration, it creates a new policy with the `HasPermissionRequirement` requirement. This requirement checks if the user has the specified permission.

### HasPermissionRequirement

The `HasPermissionRequirement` class is used to define the requirement for the `HasPermissionHandler`. It is implemented as follows:

```csharp
public class HasPermissionRequirement : IAuthorizationRequirement
{
    public Permissions Permission { get; }

    public HasPermissionRequirement(Permissions permission)
    {
        Permission = permission;
    }
}
```

The above code defines the `Permission` property, which is used to specify the required permission for the user.

### HasPermissionsAttribute

The `HasPermissionsAttribute` class is used to check if the user has the required permissions. It is implemented as follows:

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(Permissions permission)
    {
        Policy = permission.ToString();
    }
}
```

The above code sets the `Policy` property of the `AuthorizeAttribute` to the permission name. This attribute can be used on controllers or actions to check if the user has the required permission.

### SeedData

You can use a `SeedData` class is used to seed the database with initial data. It can be implemented as follows:

```csharp
```

Once you have done the above, you are now ready to create your controllers and actions. You can use the `HasPermissionAttribute` to check if the user has the required permissions.

For example, you can create a `UsersController` with the following actions:

```csharp
[ApiController]
[Route("api/[controller]")]
[HasPermission(Permissions.UsersRead)]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.UsersRead)]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    [HasPermission(Permissions.UsersWrite)]
    public async Task<IActionResult> CreateUser(CreateUserModel model)
    {
        var user = new ApplicationUser
        {
            UserName = model.UserName,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            return Ok(user);
        }

        return BadRequest(result.Errors);
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.UsersWrite)]
    public async Task<IActionResult> UpdateUser(string id, UpdateUserModel model)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.UserName = model.UserName;
        user.Email = model.Email;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return Ok(user);
        }

        return BadRequest(result.Errors);
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.UsersDelete)]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return Ok();
        }

        return BadRequest(result.Errors);
    }
}
```
