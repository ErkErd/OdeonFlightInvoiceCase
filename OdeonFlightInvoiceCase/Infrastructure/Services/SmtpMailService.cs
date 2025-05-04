using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using OdeonFlightInvoiceCase.Application.DTO;
using OdeonFlightInvoiceCase.Domain.Interfaces;

namespace OdeonFlightInvoiceCase.Infrastructure.Services.Models;

public class SmtpMailService : IMailService
{
    private readonly MailSettings _mailSettings;

    public SmtpMailService(IOptions<MailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }

    public async Task SendEmailAsync(
    MimeMessage message)
    {
        using var client = new SmtpClient();
        await client.ConnectAsync(_mailSettings.SmtpServer, _mailSettings.SmtpPort, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_mailSettings.SmtpUsername, _mailSettings.SmtpPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}