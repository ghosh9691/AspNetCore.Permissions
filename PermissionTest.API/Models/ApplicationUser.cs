using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PermissionTest.API.Models;

public class ApplicationUser : IdentityUser
{
    [StringLength(128)]
    public string FirstName { get; set; }
    
    [StringLength(128)]
    public string LastName { get; set; }
}