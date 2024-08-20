using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PermissionTest.API.Auth;
using PermissionTest.API.Models;

namespace PermissionTest.API.Controllers;

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

public class UpdateUserModel
{
    public string UserName { get; set; }
    public string Email { get; set; }
}

public class CreateUserModel
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    
}