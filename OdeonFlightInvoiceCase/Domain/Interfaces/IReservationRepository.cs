using iText.Kernel.Geom;
using OdeonFlightInvoiceCase.Domain.Entities;

namespace OdeonFlightInvoiceCase.Domain.Interfaces;

public interface IReservationRepository
{
    Task<List<Reservation>> GetReservationsByFlightAsync(DateTime flightDate, string flightNo, string carrierCode);
    Task UpdateReservationList(List<Reservation> reservationList);
} 