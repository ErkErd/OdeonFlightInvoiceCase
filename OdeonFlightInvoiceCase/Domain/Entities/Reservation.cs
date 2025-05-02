namespace OdeonFlightInvoiceCase.Domain.Entities;

public class Reservation: EntityBase
{
    public int BookingID { get; set; }
    public string Customer { get; set; }
    public string CarrierCode { get; set; }
    public string FlightNo { get; set; }//might be start with 0
    public DateTime FlightDate { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
    public decimal Price { get; set; }
    public string InvoiceNumber { get; set; }//might be start with 0
}