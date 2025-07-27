using BaseApi.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace BaseApi.Infrastructure.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendEmailAsync(string to, string cc, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        _logger.LogInformation("Sending email to {To} with subject: {Subject}", to, subject);
        
        // Simulate email sending
        await Task.Delay(100);
        
        _logger.LogInformation("Email sent successfully to {To}", to);
    }

    public async Task SendEmailAsync(string to, string cc, string subject, string body)
    {
        _logger.LogInformation("Sending email to {To}, cc: {Cc} with subject: {Subject}", to, cc, subject);
        
        // Simulate email sending
        await Task.Delay(100);
        
        _logger.LogInformation("Email sent successfully to {To}, cc: {Cc}", to, cc);
    }
} 