using OdeonFlightInvoiceCase.Application.DTO;
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
        IEnumerable<UnmatchedInvoiceDto> unmatchedRecords,
        IEnumerable<DuplicateInvoiceDto> duplicateRecords,
        IEnumerable<DifferentPricedInvoiceDto> differentPriceRecords
    );
} 