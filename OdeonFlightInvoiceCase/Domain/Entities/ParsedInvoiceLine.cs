namespace OdeonFlightInvoiceCase.Domain.Entities;

public class ParsedInvoiceLine
{
    public DateTime FlightDate { get; set; }
    public int FlightNo { get; set; }
    public int PassengerCount { get; set; }
    public decimal Price { get; set; }
    public decimal TotalPrice { get; set; }
    public int InvoiceNumber { get; set; }
} 