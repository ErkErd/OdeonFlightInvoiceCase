using AutoMapper;
using OdeonFlightInvoiceCase.Application.DTO;
using OdeonFlightInvoiceCase.Domain.Entities;
using OdeonFlightInvoiceCase.Domain.Enums;
using OdeonFlightInvoiceCase.Domain.Interfaces;

namespace OdeonFlightInvoiceCase.Application.Services;

public class InvoiceMatchService : IInvoiceMatchService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IMailService _mailService;
    private readonly IMapper _mapper;

    public InvoiceMatchService(IReservationRepository reservationRepository, IMailService mailService, IMapper mapper)
    {
        _reservationRepository = reservationRepository;
        _mailService = mailService;
        _mapper = mapper;
    }

    public async Task ProcessInvoiceAsync(string filePath, IInvoiceParser parser)
    {
        var parsedLines = await parser.ParseInvoiceAsync(filePath);
        var unmatchedRecords = new List<UnmatchedInvoiceDto>();
        var duplicateRecords = new List<DuplicateInvoiceDto>();
        var differentPriceRecords = new List<DifferentPricedInvoiceDto>();
        var removedItems = new List<Reservation>();
        var matchedCount = 0;
        int totalProcessed = 0, successfulRecords = 0, invalidRecords = 0;
        decimal totalInvoiceAmount = 0;

        
        foreach (var line in parsedLines)
        {

            var reservations = await _reservationRepository.GetReservationsByFlightAsync(line.FlightDate, line.FlightNo, line.CarrierCode);
            
            totalProcessed++;
            var invoiceNullSamePriceReservations = reservations
                                                    .Where(x => string.IsNullOrEmpty(x.InvoiceNumber) &&  x.Price == line.Price)
                                                    .Take(line.PassengerCount)
                                                    .ToList();
            if (invoiceNullSamePriceReservations != null && invoiceNullSamePriceReservations.Count == line.PassengerCount)//linde daki sayý kadar ayný fiyatta faturasýz koltuk var mý
            {
                matchedCount++;
                //Success conditions are met
                invoiceNullSamePriceReservations.ForEach(x =>
                {
                    successfulRecords++;
                    x.InvoiceNumber = line.InvoiceNumber;
                });

                await _reservationRepository.UpdateReservationList(invoiceNullSamePriceReservations);
                continue;
            }

            var samePriceReservations = reservations
                                        .Where(x =>x.Price == line.Price)
                                        .Take(line.PassengerCount)
                                        .ToList();
            if (samePriceReservations != null && samePriceReservations.Count == line.PassengerCount)//invoice null olan yeterince yok. Toplamda ayný fiyattan var mý ? 
            {
                //Duplicate conditions are met
                duplicateRecords.Add(_mapper.Map<DuplicateInvoiceDto>(line));
                continue;
               
            }

            var invoiceNullDifferentPriceReservations = reservations
                                                        .Where(x => string.IsNullOrEmpty(x.InvoiceNumber))
                                                        .GroupBy(x => x.Price)
                                                        .FirstOrDefault(g => g.Count() == line.PassengerCount);
            if (invoiceNullDifferentPriceReservations != null)
            {
                //Different priced conditions are met
                var differentPricedInvoice = _mapper.Map<DifferentPricedInvoiceDto>(line);
                differentPricedInvoice.DbPrice = invoiceNullDifferentPriceReservations.First().Price;
                differentPriceRecords.Add(differentPricedInvoice);
                continue;
            }

            //Rest are unmatched
            unmatchedRecords.Add(_mapper.Map<UnmatchedInvoiceDto>(line));
            invalidRecords++;

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
