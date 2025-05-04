using AutoMapper;
using OdeonFlightInvoiceCase.Application.DTO;
using OdeonFlightInvoiceCase.Domain.Interfaces;

namespace OdeonFlightInvoiceCase.Application.Services;

public class InvoiceMatchService : IInvoiceMatchService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IInvoiceMatchedNotifier _notifier;
    private readonly IMapper _mapper;

    public InvoiceMatchService(IReservationRepository reservationRepository, IInvoiceMatchedNotifier notifier, IMailService mailService, IMapper mapper)
    {
        _reservationRepository = reservationRepository;
        _notifier = notifier;
        _mapper = mapper;
    }

    public async Task ProcessInvoiceAsync(string filePath, IInvoiceParser parser)
    {
        var parsedLines = await parser.ParseInvoiceAsync(filePath);
        var unmatchedRecords = new List<UnmatchedInvoiceDto>();
        var duplicateRecords = new List<DuplicateInvoiceDto>();
        var differentPriceRecords = new List<DifferentPricedInvoiceDto>();

        int successfulRecords = 0, matchedCount = 0;

        if (!parsedLines.Any())
        {
            return;
        }
        var invoiceId = parsedLines.First().InvoiceNumber;

        foreach (var line in parsedLines)
        {
            var reservations = await _reservationRepository.GetReservationsByFlightAsync(
                line.FlightDate, line.FlightNo, line.CarrierCode);

            var invoiceNullSamePriceReservations = reservations
                .Where(x => string.IsNullOrEmpty(x.InvoiceNumber) && x.Price == line.Price)
                .Take(line.PassengerCount)
                .ToList();
            //Tolist always returns a list, even if empty, not necessary to check for null
            if (invoiceNullSamePriceReservations.Count == line.PassengerCount)
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
                .Where(x => x.Price == line.Price)
                .Take(line.PassengerCount)
                .ToList();
            //invoice null olan yeterince yok. Toplamda aynı fiyattan var mı ? 
            if (samePriceReservations.Count == line.PassengerCount)
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
        }

        var result = new MatchInvoiceResultDto
        {
            TotalProcessed = parsedLines.Count(),
            SuccessfulRecords = successfulRecords,
            MatchedCount = matchedCount,
            InvoiceId = invoiceId,
            UnmatchedRecords = unmatchedRecords,
            DuplicateRecords = duplicateRecords,
            DifferentPriceRecords = differentPriceRecords
        };

        await _notifier.NotifyAsync(result);//might be event
    }
}

