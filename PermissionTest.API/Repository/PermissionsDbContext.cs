using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PermissionTest.API.Models;

namespace PermissionTest.API.Repository;

public class PermissionsDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public PermissionsDbContext(DbContextOptions<PermissionsDbContext> options) : base(options)
    {
    }    
}