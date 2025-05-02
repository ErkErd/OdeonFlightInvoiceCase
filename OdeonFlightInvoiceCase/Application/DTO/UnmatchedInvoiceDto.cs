using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdeonFlightInvoiceCase.Application.DTO
{
    public class UnmatchedInvoiceDto
    {
        public DateTime FlightDate { get; set; }
        public string CarrierCode { get; set; }
        public string FlightNo { get; set; }
        public int PassengerCount { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public string InvoiceNumber { get; set; }
    }
}
