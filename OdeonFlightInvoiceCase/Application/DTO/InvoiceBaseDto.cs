using System.Globalization;
using System.Text;

namespace OdeonFlightInvoiceCase.Application.DTO;

public abstract class InvoiceBaseDto
{
    public DateTime FlightDate { get; set; }
    public string CarrierCode { get; set; }
    public string FlightNo { get; set; }//might be start with 0
    public int PassengerCount { get; set; }
    public decimal Price { get; set; }
    public decimal TotalPrice { get; set; }
    public string InvoiceNumber { get; set; }//might be start with 0
    public abstract byte[] GenerateCsv(IEnumerable<InvoiceBaseDto> records);
    public string EscapeCsv(object value)
    {
        if (value == null) return "\"\"";

        if (value is decimal decimalValue)
            return $"\"{decimalValue.ToString(CultureInfo.InvariantCulture)}\"";

        if (value is double doubleValue)
            return $"\"{doubleValue.ToString(CultureInfo.InvariantCulture)}\"";

        if (value is float floatValue)
            return $"\"{floatValue.ToString(CultureInfo.InvariantCulture)}\"";

        var str = value.ToString().Replace("\"", "\"\"");
        return $"\"{str}\"";
    }
} 