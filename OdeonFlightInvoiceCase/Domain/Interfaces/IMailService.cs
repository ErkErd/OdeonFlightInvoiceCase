using OdeonFlightInvoiceCase.Domain.Entities;

namespace OdeonFlightInvoiceCase.Domain.Interfaces;

public interface IMailService
{
    Task SendInvoiceProcessingSummaryAsync(
        int totalProcessed,
        int matchedCount,
        int unmatchedCount,
        int duplicateCount,
        int differentPriceCount,
        IEnumerable<ParsedInvoiceLine> unmatchedRecords,
        IEnumerable<ParsedInvoiceLine> duplicateRecords,
        IEnumerable<ParsedInvoiceLine> differentPriceRecords
    );
} 