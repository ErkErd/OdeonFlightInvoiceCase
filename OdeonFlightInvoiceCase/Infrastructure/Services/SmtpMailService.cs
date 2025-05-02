using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using OdeonFlightInvoiceCase.Application.DTO;
using OdeonFlightInvoiceCase.Domain.Entities;
using OdeonFlightInvoiceCase.Domain.Interfaces;

namespace OdeonFlightInvoiceCase.Infrastructure.Services;

public class SmtpMailService : IMailService
{
    private readonly MailSettings _mailSettings;

    public SmtpMailService(IOptions<MailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }

    public async Task SendInvoiceProcessingSummaryAsync(
        int totalProcessed,
        int matchedCount,
        int unmatchedCount,
        int duplicateCount,
        int differentPriceCount,
        IEnumerable<UnmatchedInvoiceDto> unmatchedRecords,
        IEnumerable<DuplicateInvoiceDto> duplicateRecords,
        IEnumerable<DifferentPricedInvoiceDto> differentPriceRecords)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Invoice Processor", _mailSettings.FromEmail));
        message.To.Add(new MailboxAddress("Recipient", _mailSettings.ToEmail));
        message.Subject = "Invoice Processing Summary";

        var bodyBuilder = new BodyBuilder
        {
            TextBody = GenerateSummaryText(totalProcessed, matchedCount, unmatchedCount, duplicateCount, differentPriceCount)
        };

        //Add CSV attachments
        //if (unmatchedRecords.Any())
        //    bodyBuilder.Attachments.Add("unmatched.csv", GenerateCsv(unmatchedRecords));
        //if (duplicateRecords.Any())
        //    bodyBuilder.Attachments.Add("duplicate.csv", GenerateCsv(duplicateRecords));
        //if (differentPriceRecords.Any())
        //    bodyBuilder.Attachments.Add("different_price.csv", GenerateCsv(differentPriceRecords));

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        //await client.ConnectAsync(_mailSettings.SmtpServer, _mailSettings.SmtpPort, SecureSocketOptions.StartTls);
        //await client.AuthenticateAsync(_mailSettings.SmtpUsername, _mailSettings.SmtpPassword);
        //await client.SendAsync(message);
        //await client.DisconnectAsync(true);
    }

    private string GenerateSummaryText(int totalProcessed, int matchedCount, int unmatchedCount, int duplicateCount, int differentPriceCount)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Invoice Processing Summary");
        sb.AppendLine("------------------------");
        sb.AppendLine($"Total Processed: {totalProcessed}");
        sb.AppendLine($"Successfully Matched: {matchedCount}");
        sb.AppendLine($"Unmatched Records: {unmatchedCount}");
        sb.AppendLine($"Duplicate Invoices: {duplicateCount}");
        sb.AppendLine($"Different Price Records: {differentPriceCount}");
        return sb.ToString();
    }

    private byte[] GenerateCsv(IEnumerable<ParsedInvoiceLine> records)
    {
        var sb = new StringBuilder();
        sb.AppendLine("FlightDate,FlightNumber,PassengerCount,Price,TotalPrice,InvoiceNumber");
        
        foreach (var record in records)
        {
            sb.AppendLine($"{record.FlightDate:yyyy-MM-dd},{record.FlightNo},{record.PassengerCount},{record.Price},{record.TotalPrice},{record.InvoiceNumber}");
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}

public class MailSettings
{
    public string SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public string SmtpUsername { get; set; }
    public string SmtpPassword { get; set; }
    public string FromEmail { get; set; }
    public string ToEmail { get; set; }
} 