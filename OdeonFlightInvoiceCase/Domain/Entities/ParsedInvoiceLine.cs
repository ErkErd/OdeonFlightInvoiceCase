namespace OdeonFlightInvoiceCase.Domain.Entities;

public class ParsedInvoiceLine
{
    public DateTime FlightDate { get; set; }
    public string CarrierCode { get; set; }
    public string FlightNo { get; set; }//might be start with 0
    public int PassengerCount { get; set; }
    public decimal Price { get; set; }
    public decimal TotalPrice { get; set; }
    public string InvoiceNumber { get; set; }//might be start with 0
} 