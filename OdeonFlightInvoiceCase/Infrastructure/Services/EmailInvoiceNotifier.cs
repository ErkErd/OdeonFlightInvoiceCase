using System.Text;
using MimeKit;
using Microsoft.Extensions.Options;
using OdeonFlightInvoiceCase.Application.DTO;
using OdeonFlightInvoiceCase.Application.Interfaces;
using OdeonFlightInvoiceCase.Domain.Interfaces;
using OdeonFlightInvoiceCase.Infrastructure.Services.Models;

namespace OdeonFlightInvoiceCase.Infrastructure.Services;

public class EmailInvoiceNotifier : IInvoiceMatchedNotifier
{
    private readonly IMailService _mailService;
    private readonly MailSettings _mailSettings;

    public EmailInvoiceNotifier(IMailService mailService, IOptions<MailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
        _mailService = mailService;
    }

    public async Task NotifyAsync(MatchInvoiceResultDto result)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Invoice Processor", _mailSettings.FromEmail));
        message.To.Add(new MailboxAddress("Recipient", _mailSettings.ToEmail));
        message.Subject = "Invoice Processing Summary Invoice Number = "+ result.InvoiceId;

        var bodyBuilder = new BodyBuilder
        {
            TextBody = GenerateSummaryText(result.TotalProcessed, result.SuccessfulRecords, result.MatchedCount, result.UnmatchedRecords.Count(), result.DuplicateRecords.Count(), result.DifferentPriceRecords.Count())
        };

        if (result.UnmatchedRecords.Any())
            bodyBuilder.Attachments.Add(result.InvoiceId + "_unmatched.csv", result.UnmatchedRecords.First().GenerateCsv(result.UnmatchedRecords));

        if (result.DuplicateRecords.Any())
            bodyBuilder.Attachments.Add(result.InvoiceId + "_duplicate.csv", result.DuplicateRecords.First().GenerateCsv(result.DuplicateRecords));

        if (result.DifferentPriceRecords.Any())
            bodyBuilder.Attachments.Add(result.InvoiceId + "_different_price.csv", result.DifferentPriceRecords.First().GenerateCsv(result.DifferentPriceRecords));


        message.Body = bodyBuilder.ToMessageBody();

        await _mailService.SendEmailAsync(message);
    }
    private string GenerateSummaryText(int totalProcessed,int successfullRecods, int matchedCount, int unmatchedCount, int duplicateCount, int differentPriceCount)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Invoice Processing Summary");
        sb.AppendLine("------------------------");
        sb.AppendLine($"Total Processed: {totalProcessed}");
        sb.AppendLine($"Successfully Matched (Database processed record count): {successfullRecods}");
        sb.AppendLine($"Matched Records: {matchedCount}");
        sb.AppendLine($"Unmatched Records: {unmatchedCount}");
        sb.AppendLine($"Duplicate Invoices: {duplicateCount}");
        sb.AppendLine($"Different Price Records: {differentPriceCount}");
        return sb.ToString();
    }
}