using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using OdeonFlightInvoiceCase.Domain.Entities;
using OdeonFlightInvoiceCase.Domain.Interfaces;

namespace OdeonFlightInvoiceCase.Infrastructure.Services;

public class PdfInvoiceParser : IInvoiceParser
{
    public async Task<IEnumerable<ParsedInvoiceLine>> ParseInvoiceAsync(string filePath)
    {
        var result = new List<ParsedInvoiceLine>();
        int invoiceNumber = 0;

        using (var pdfReader = new PdfReader(filePath))
        using (var pdfDocument = new PdfDocument(pdfReader))
        {
            for (int page = 1; page <= pdfDocument.GetNumberOfPages(); page++)
            {
                var strategy = new LocationTextExtractionStrategy();
                var currentText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(page), strategy);

                // Invoice number extraction
                if (invoiceNumber == 0)
                {
                    var invoiceNumberMatch = System.Text.RegularExpressions.Regex.Match(currentText, @"Nummer:\s*(\d+)");
                    if (invoiceNumberMatch.Success)
                    {
                        invoiceNumber = int.Parse(invoiceNumberMatch.Groups[1].Value);
                    }
                }

                // Parse table rows
                var lines = currentText.Split('\n');
                foreach (var line in lines)
                {
                    if (line.Contains("Flugdatum") || line.Contains("Flug Nr") || line.Contains("Anzahl") || line.Contains("Einzelpreis") || line.Contains("Betrag"))
                        continue;

                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 5)
                    {
                        if (DateTime.TryParse(parts[0], out DateTime flightDate))
                        {
                            if (int.TryParse(parts[1], out int flightNo))
                            {
                                if (int.TryParse(parts[2].Replace("-", ""), out int passengerCount))
                                {
                                    if (decimal.TryParse(parts[3], out decimal price))
                                    {
                                        if (decimal.TryParse(parts[4], out decimal totalPrice))
                                        {

                                            result.Add(new ParsedInvoiceLine
                                            {
                                                FlightDate = flightDate,
                                                FlightNo = flightNo,
                                                PassengerCount = passengerCount,
                                                Price = price,
                                                TotalPrice = totalPrice,
                                                InvoiceNumber = invoiceNumber
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return result;
    }
}