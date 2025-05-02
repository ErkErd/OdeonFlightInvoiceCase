using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdeonFlightInvoiceCase.Application.DTO
{
    public class DuplicateInvoiceDto : InvoiceBaseDto
    {
        public override byte[] GenerateCsv(IEnumerable<InvoiceBaseDto> records)
        {
            var csv = new StringBuilder();
            csv.AppendLine("InvoiceNumber" +
                            ",FlightDate" +
                            ",CarrierCode" +
                            ",FlightNumber" +
                            ",PassengerCount" +
                            ",Price" +
                            ",TotalPrice");
            foreach (var record in records.Cast<DuplicateInvoiceDto>())
            {
                csv.AppendLine(string.Join(",", new[]
                {
                    EscapeCsv(record.InvoiceNumber),
                    EscapeCsv(record.FlightDate.ToString("yyyy-MM-dd")),
                    EscapeCsv(record.CarrierCode),
                    EscapeCsv(record.FlightNo),
                    EscapeCsv(record.PassengerCount),
                    EscapeCsv(record.Price),
                    EscapeCsv(record.TotalPrice)
                }));
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }
    }
}
