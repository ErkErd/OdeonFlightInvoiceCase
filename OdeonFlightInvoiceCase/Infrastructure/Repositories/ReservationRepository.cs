using Microsoft.EntityFrameworkCore;
using OdeonFlightInvoiceCase.Domain.Entities;
using OdeonFlightInvoiceCase.Domain.Interfaces;
using OdeonFlightInvoiceCase.Infrastructure.Data;

namespace OdeonFlightInvoiceCase.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly ApplicationDbContext _context;

    public ReservationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Reservation>> GetReservationsByFlightAsync(DateTime flightDate, int flightNo)
    {
        return await _context.Reservations
            .Where(r => r.FlightDate.Date == flightDate.Date && r.FlightNo == flightNo)
            .ToListAsync();
    }

    public async Task UpdateReservationInvoiceNumberAsync(int reservationId, int invoiceNumber)
    {
        var reservation = await _context.Reservations.FindAsync(reservationId);
        if (reservation != null)
        {
            reservation.InvoiceNumber = invoiceNumber;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsInvoiceNumberExistsAsync(int invoiceNumber)
    {
        return await _context.Reservations
            .AnyAsync(r => r.InvoiceNumber == invoiceNumber);
    }
} 