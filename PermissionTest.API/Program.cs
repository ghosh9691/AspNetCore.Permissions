using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PermissionTest.API.Auth;
using PermissionTest.API.Interfaces;
using PermissionTest.API.Models;
using PermissionTest.API.Repository;
using PermissionTest.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PermissionsDbContext>(options => options.UseSqlite(connString));

builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<PermissionsDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication();

builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddTransient<IEmailSender<ApplicationUser>, AuthEmailSender>();

builder.Services.AddScoped<IAuthorizationHandler, HasPermissionHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionsAuthorizationPolicyProvider>();
    
builder.Services.AddAuthorization();


builder.Services.AddControllers().AddControllersAsServices();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

RegisterServices(builder.Services);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGroup("api/auth").MapIdentityApi<ApplicationUser>();


app.Run();


void RegisterServices(IServiceCollection services)
{
    
}
