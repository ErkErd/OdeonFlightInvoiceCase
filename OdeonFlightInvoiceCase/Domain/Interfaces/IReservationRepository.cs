using OdeonFlightInvoiceCase.Domain.Entities;

namespace OdeonFlightInvoiceCase.Domain.Interfaces;

public interface IReservationRepository
{
    Task<IEnumerable<Reservation>> GetReservationsByFlightAsync(DateTime flightDate, int flightNo);
    Task UpdateReservationInvoiceNumberAsync(int reservationId, int invoiceNumber);
    Task<bool> IsInvoiceNumberExistsAsync(int invoiceNumber);
} 