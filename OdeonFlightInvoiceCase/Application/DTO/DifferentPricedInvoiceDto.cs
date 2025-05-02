using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdeonFlightInvoiceCase.Application.DTO
{
    public class DifferentPricedInvoiceDto
    {
        public DateTime FlightDate { get; set; }
        public string CarrierCode { get; set; }
        public string FlightNo { get; set; }
        public int PassengerCount { get; set; }
        public int BookingId { get; set; }
        public decimal DbPrice { get; set; }
        public decimal PdfPrice { get; set; }
        public string InvoiceNumber { get; set; }
    }
}
