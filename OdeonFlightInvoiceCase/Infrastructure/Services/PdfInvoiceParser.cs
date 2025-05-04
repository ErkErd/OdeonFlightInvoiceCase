using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using OdeonFlightInvoiceCase.Domain.Entities;
using OdeonFlightInvoiceCase.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace OdeonFlightInvoiceCase.Infrastructure.Services;

public class PdfInvoiceParser : IInvoiceParser
{
    public async Task<IEnumerable<ParsedInvoiceLine>> ParseInvoiceAsync(string filePath)
    {
        var result = new List<ParsedInvoiceLine>();
        var invoiceNumber = string.Empty;

        using (var pdfReader = new PdfReader(filePath))
        using (var pdfDocument = new PdfDocument(pdfReader))
        {
            for (int page = 1; page <= pdfDocument.GetNumberOfPages(); page++)
            {
                var strategy = new LocationTextExtractionStrategy();
                var currentText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(page), strategy);

                // Invoice number extraction
                if (string.IsNullOrEmpty(invoiceNumber))
                {
                    var invoiceNumberMatch = Regex.Match(currentText, @"Nummer\s+Seite\s+Datum\s*\r?\n(\d+)");
                    if (invoiceNumberMatch.Success)
                    {
                        invoiceNumber = invoiceNumberMatch.Groups[1].Value;
                    }
                }

                // Parse table rows
                var lines = currentText.Split('\n');
                var findHeaders = false;

                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (!findHeaders && line.Contains("Flugdatum") || line.Contains("Flug Nr") || line.Contains("Anzahl") || line.Contains("Einzelpreis") || line.Contains("Betrag"))
                    {
                        findHeaders = true;
                        continue;
                    }
                    if (!findHeaders)
                    {
                        continue;
                    }
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 10 && !parts[7].Contains('-'))
                    {
                        if (DateTime.TryParse(parts[2], out DateTime flightDate))
                        {
                            if (int.TryParse(parts[7], out int passengerCount))
                            {
                                if (decimal.TryParse(parts[8], out decimal price))
                                {
                                    if (decimal.TryParse(parts[9], out decimal totalPrice))
                                    {
                                        result.Add(new ParsedInvoiceLine
                                        {
                                            FlightDate = flightDate,
                                            FlightNo = parts[4],
                                            CarrierCode = parts[3],
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
        return result;
    }
}