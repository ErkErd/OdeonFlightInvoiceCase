using OdeonFlightInvoiceCase.Domain.Entities;

namespace OdeonFlightInvoiceCase.Domain.Interfaces;

public interface IInvoiceParser
{
    Task<IEnumerable<ParsedInvoiceLine>> ParseInvoiceAsync(string filePath);
} 