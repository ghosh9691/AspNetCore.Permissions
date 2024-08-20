using PermissionTest.API.Interfaces;

namespace PermissionTest.API.Services;

public class EmailService : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string message)
    {
        Console.WriteLine($"Sending email to {to} with subject {subject} and message {message}");
    }
}