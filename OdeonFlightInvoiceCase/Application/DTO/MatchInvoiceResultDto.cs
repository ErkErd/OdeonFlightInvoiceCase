
namespace OdeonFlightInvoiceCase.Application.DTO;

public class MatchInvoiceResultDto
{
    public int TotalProcessed { get; set; }
    public int SuccessfulRecords { get; set; }
    public int MatchedCount { get; set; }
    public string InvoiceId { get; set; }
    public IEnumerable<UnmatchedInvoiceDto> UnmatchedRecords { get; set; }
    public IEnumerable<DuplicateInvoiceDto> DuplicateRecords { get; set; }
    public IEnumerable<DifferentPricedInvoiceDto> DifferentPriceRecords { get; set; }
} 