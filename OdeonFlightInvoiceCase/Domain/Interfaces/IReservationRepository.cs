using iText.Kernel.Geom;
using OdeonFlightInvoiceCase.Domain.Entities;

namespace OdeonFlightInvoiceCase.Domain.Interfaces;

public interface IReservationRepository
{
    Task<List<Reservation>> GetReservationsByFlightAsync(DateTime flightDate, string flightNo, string carrierCode);
    Task UpdateReservationInvoiceNumberAsync(int reservationId, string invoiceNumber);
    Task UpdateReservationList(List<Reservation> reservationList);
    Task<bool> IsInvoiceNumberExistsAsync(string invoiceNumber);
} 