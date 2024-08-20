using System.Text;
using Microsoft.AspNetCore.Identity;
using PermissionTest.API.Interfaces;
using PermissionTest.API.Models;

namespace PermissionTest.API.Auth;

public class AuthEmailSender : IEmailSender<ApplicationUser>
{
    private readonly IEmailService _emailService;
    
    public AuthEmailSender(IEmailService emailService)
    {
        _emailService = emailService;
    }
    
    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        var sb = new StringBuilder();
        sb.AppendLine($@"<div>Dear {user.FirstName},</div>");
        sb.AppendLine("<br/>");
        sb.AppendLine(
            "<div>You have recently created an account on Pyxis Hotelier platform. If you did actually request the user,");
        sb.AppendLine("please click on this link below to activate your account.</div>");
        sb.AppendLine("<br/>");
        sb.AppendLine("<div><b>Note:</b> If you did not request the account creation, please do not click this link.</div>");
        sb.AppendLine("<br/>");
        sb.AppendLine($@"<div><a href=""{confirmationLink}"">Activate your email</a></div>");
        await _emailService.SendEmailAsync(email, "Confirm your Pyxis Hotelier Account", sb.ToString());
    }

    public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        var sb = new StringBuilder();
        sb.AppendLine($@"<div>Dear {user.FirstName},</div>");
        sb.AppendLine("<br/>");
        sb.AppendLine(
            "<div>You have a password reset for your Pyxis Hotelier platform account. If you did actually request the user,");
        sb.AppendLine("please click on this link below to reset your password.</div>");
        sb.AppendLine("<br/>");
        sb.AppendLine("<div><b>Note:</b> If you did not request the password reset, please do not click this link.</div>");
        sb.AppendLine("<br/>");
        sb.AppendLine($@"<div><a href=\""{resetLink}\"">Reset your password</a></div>");
        await _emailService.SendEmailAsync(email, "Reset your Pyxis Hotelier Password", sb.ToString());
    }

    public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        var sb = new StringBuilder();
        sb.AppendLine($@"<div>Dear {user.FirstName},</div>");
        sb.AppendLine("<br/>");
        sb.AppendLine(
            "<div>Here is your password reset code. Please do not share this code with anyone else.</div>");
        sb.AppendLine("<br/>");
        sb.AppendLine("<div><b>Note:</b> If you did not request a password reset, please do not use this code.</div>");
        sb.AppendLine("<br/>");
        sb.AppendLine($@"<div><b>{resetCode}</b></div>");
        await _emailService.SendEmailAsync(email, "Your Pyxis Hotelier Password Reset Code", sb.ToString());
    }
}