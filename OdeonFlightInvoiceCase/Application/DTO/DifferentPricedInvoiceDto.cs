using System.Text;

namespace OdeonFlightInvoiceCase.Application.DTO
{
    public class DifferentPricedInvoiceDto : InvoiceBaseDto
    {
        public decimal DbPrice { get; set; }
        public decimal PdfPrice { get; set; }

        public override byte[] GenerateCsv(IEnumerable<InvoiceBaseDto> records)
        {
            var csv = new StringBuilder();
            csv.AppendLine(string.Join(",", new[]
            {
                "InvoiceNumber",
                "FlightDate",
                "CarrierCode",
                "FlightNumber",
                "PassengerCount",
                "PdfPrice",
                "DbPrice",
                "PdfTotalPrice"
            }));
            foreach (var record in records.Cast<DifferentPricedInvoiceDto>())
            {
                csv.AppendLine(string.Join(",", new[]
                {
                    EscapeCsv(record.InvoiceNumber),
                    EscapeCsv(record.FlightDate.ToString("yyyy-MM-dd")),
                    EscapeCsv(record.CarrierCode),
                    EscapeCsv(record.FlightNo),
                    EscapeCsv(record.PassengerCount),
                    EscapeCsv(record.PdfPrice),
                    EscapeCsv(record.DbPrice),
                    EscapeCsv(record.TotalPrice)
                }));
            }
            return Encoding.UTF8.GetBytes(csv.ToString());
        }
    }
}
