using OdeonFlightInvoiceCase.Domain.Entities;
using OdeonFlightInvoiceCase.Domain.Enums;
using OdeonFlightInvoiceCase.Domain.Interfaces;

namespace OdeonFlightInvoiceCase.Application.Services;

public class InvoiceMatchService : IInvoiceMatchService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IMailService _mailService;

    public InvoiceMatchService(IReservationRepository reservationRepository, IMailService mailService)
    {
        _reservationRepository = reservationRepository;
        _mailService = mailService;
    }

    public async Task ProcessInvoiceAsync(string filePath, IInvoiceParser parser)
    {
        var parsedLines = await parser.ParseInvoiceAsync(filePath);
        var unmatchedRecords = new List<ParsedInvoiceLine>();
        var duplicateRecords = new List<ParsedInvoiceLine>();
        var differentPriceRecords = new List<ParsedInvoiceLine>();
        var matchedCount = 0;

        foreach (var line in parsedLines)
        {

            var reservations = await _reservationRepository.GetReservationsByFlightAsync(line.FlightDate, line.FlightNo);
            
            if (!reservations.Any())
            {
                unmatchedRecords.Add(line);
                continue;
            }

            var isDuplicate = await _reservationRepository.IsInvoiceNumberExistsAsync(line.InvoiceNumber);
            if (isDuplicate)
            {
                duplicateRecords.Add(line);
                continue;
            }

            var matchingReservation = reservations.FirstOrDefault(r => r.Price == line.Price);
            if (matchingReservation == null)
            {
                differentPriceRecords.Add(line);
                continue;
            }

            await _reservationRepository.UpdateReservationInvoiceNumberAsync(matchingReservation.Id, line.InvoiceNumber);
            matchedCount++;
        }

        await _mailService.SendInvoiceProcessingSummaryAsync(
            parsedLines.Count(),
            matchedCount,
            unmatchedRecords.Count,
            duplicateRecords.Count,
            differentPriceRecords.Count,
            unmatchedRecords,
            duplicateRecords,
            differentPriceRecords
        );
    }
}

public interface IInvoiceMatchService
{
    Task ProcessInvoiceAsync(string filePath, IInvoiceParser parser);
} 